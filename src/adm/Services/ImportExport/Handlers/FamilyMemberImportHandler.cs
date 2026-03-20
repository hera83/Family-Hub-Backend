using ClosedXML.Excel;
using FamilyHub.Adm.Infrastructure.Clients.Calendar;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Api.Calendar;
using FamilyHub.Adm.Services.ImportExport.Models;
using System.Net;

namespace FamilyHub.Adm.Services.ImportExport.Handlers;

public sealed class FamilyMemberImportHandler(ICalendarApiClient calendarApiClient)
    : ImportHandlerBase, IImportHandler
{
    public string ImportType => ImportTypeNames.FamilyMembers;
    public string DisplayName => ImportTypeNames.GetDisplayName(ImportType);

    // Import row type. Adjust fields here if the column format changes.
    private sealed class RowData
    {
        public Guid? Id { get; init; }
        public required string Name { get; init; }
        public required string Color { get; init; }
    }

    public ImportPreview Parse(IXLWorkbook workbook)
    {
        var sheet = FindSheet(workbook, "FamilyMembers");
        var map = ReadHeaderMap(sheet);
        var rows = new List<ImportPreviewRow>();

        foreach (var xlRow in sheet.RowsUsed().Skip(1))
        {
            var rowNum = xlRow.RowNumber();
            var id   = GetCellString(xlRow, map, "Id");
            var name = GetCellString(xlRow, map, "Name");
            var color = GetCellString(xlRow, map, "Color");

            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(name))  errors.Add("Name er påkrævet.");
            if (string.IsNullOrWhiteSpace(color)) errors.Add("Color er påkrævet.");

            rows.Add(new ImportPreviewRow
            {
                RowNumber = rowNum,
                IsValid = errors.Count == 0,
                Errors = errors,
                DisplayColumns = [D("Id", id), D("Name", name), D("Color", color)],
                Data = errors.Count == 0
                    ? new RowData { Id = ParseGuid(id), Name = name!, Color = color! }
                    : null
            });
        }

        return new ImportPreview
        {
            TypeName = ImportType,
            DisplayName = DisplayName,
            TotalRows = rows.Count,
            ValidRows = rows.Count(r => r.IsValid),
            InvalidRows = rows.Count(r => !r.IsValid),
            Rows = rows
        };
    }

    public async Task<ImportResult> ExecuteAsync(ImportPreview preview, CancellationToken cancellationToken)
    {
        // Pre-fetch existing IDs for upsert decision.
        var existingIds = new HashSet<Guid>();
        try
        {
            var all = await calendarApiClient.GetFamilyMembersAsync(cancellationToken);
            foreach (var m in all) existingIds.Add(m.Id);
        }
        catch (ApiClientException) { /* proceed; all rows will be created */ }

        int created = 0, updated = 0, failed = 0;
        var errors = new List<ImportRowError>();

        foreach (var row in preview.Rows.Where(r => r.IsValid))
        {
            var data = (RowData)row.Data!;
            try
            {
                if (data.Id.HasValue && existingIds.Contains(data.Id.Value))
                {
                    await calendarApiClient.UpdateFamilyMemberAsync(data.Id.Value,
                        new UpdateFamilyMemberRequest { Name = data.Name, Color = data.Color },
                        cancellationToken);
                    updated++;
                }
                else
                {
                    await calendarApiClient.CreateFamilyMemberAsync(
                        new CreateFamilyMemberRequest { Name = data.Name, Color = data.Color },
                        cancellationToken);
                    created++;
                }
            }
            catch (ApiClientException ex)
            {
                failed++;
                errors.Add(new ImportRowError { RowNumber = row.RowNumber, Message = ex.UserMessage });
            }
        }

        return new ImportResult
        {
            TypeName = ImportType,
            DisplayName = DisplayName,
            TotalRead = preview.TotalRows,
            Created = created,
            Updated = updated,
            Failed = failed,
            Errors = errors
        };
    }
}
