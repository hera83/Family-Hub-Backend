using ClosedXML.Excel;
using FamilyHub.Adm.Infrastructure.Clients.Calendar;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Api.Calendar;
using FamilyHub.Adm.Services.ImportExport.Models;

namespace FamilyHub.Adm.Services.ImportExport.Handlers;

public sealed class CalendarEventImportHandler(ICalendarApiClient calendarApiClient)
    : ImportHandlerBase, IImportHandler
{
    public string ImportType => ImportTypeNames.CalendarEvents;
    public string DisplayName => ImportTypeNames.GetDisplayName(ImportType);

    // Adjust import rules for calendar events here.
    private sealed class RowData
    {
        public Guid? Id { get; init; }
        public required string Title { get; init; }
        public string? Description { get; init; }
        public required DateOnly EventDate { get; init; }
        public TimeOnly? StartTime { get; init; }
        public TimeOnly? EndTime { get; init; }
        public Guid? FamilyMemberId { get; init; }
        public string? FamilyMemberName { get; init; }
        public string? RecurrenceType { get; init; }
        public int[]? RecurrenceDays { get; init; }
    }

    public ImportPreview Parse(IXLWorkbook workbook)
    {
        var sheet = FindSheet(workbook, "CalendarEvents");
        var map = ReadHeaderMap(sheet);
        var rows = new List<ImportPreviewRow>();

        foreach (var xlRow in sheet.RowsUsed().Skip(1))
        {
            var rowNum = xlRow.RowNumber();
            var id              = GetCellString(xlRow, map, "Id");
            var title           = GetCellString(xlRow, map, "Title");
            var description     = GetCellString(xlRow, map, "Description");
            var eventDateStr    = GetCellString(xlRow, map, "EventDate");
            var startTimeStr    = GetCellString(xlRow, map, "StartTime");
            var endTimeStr      = GetCellString(xlRow, map, "EndTime");
            var familyMemberId  = GetCellString(xlRow, map, "FamilyMemberId");
            var familyMemberName = GetCellString(xlRow, map, "FamilyMemberName");
            var recurrenceType  = GetCellString(xlRow, map, "RecurrenceType");
            var recurrenceDays  = GetCellString(xlRow, map, "RecurrenceDays");

            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(title)) errors.Add("Title er påkrævet.");
            var parsedDate = ParseDateOnly(eventDateStr);
            if (parsedDate is null) errors.Add("EventDate er påkrævet og skal være en gyldig dato (yyyy-MM-dd).");

            rows.Add(new ImportPreviewRow
            {
                RowNumber = rowNum,
                IsValid = errors.Count == 0,
                Errors = errors,
                DisplayColumns =
                [
                    D("Id", id),
                    D("Title", title),
                    D("EventDate", eventDateStr),
                    D("FamilyMemberName", familyMemberName),
                    D("RecurrenceType", recurrenceType),
                ],
                Data = errors.Count == 0
                    ? new RowData
                    {
                        Id = ParseGuid(id),
                        Title = title!,
                        Description = description,
                        EventDate = parsedDate!.Value,
                        StartTime = ParseTimeOnly(startTimeStr),
                        EndTime = ParseTimeOnly(endTimeStr),
                        FamilyMemberId = ParseGuid(familyMemberId),
                        FamilyMemberName = familyMemberName,
                        RecurrenceType = recurrenceType,
                        RecurrenceDays = ParseIntArray(recurrenceDays),
                    }
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
        // Pre-fetch family members for name → ID lookup.
        var membersByName = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        var existingEventIds = new HashSet<Guid>();
        try
        {
            var members = await calendarApiClient.GetFamilyMembersAsync(cancellationToken);
            foreach (var m in members) membersByName[m.Name] = m.Id;

            var events = await calendarApiClient.GetCalendarEventsAsync(null, cancellationToken);
            foreach (var e in events) existingEventIds.Add(e.Id);
        }
        catch (ApiClientException) { }

        int created = 0, updated = 0, failed = 0;
        var errors = new List<ImportRowError>();

        foreach (var row in preview.Rows.Where(r => r.IsValid))
        {
            var data = (RowData)row.Data!;

            // Resolve FamilyMemberId: use provided ID first, fall back to name lookup.
            var resolvedMemberId = data.FamilyMemberId;
            if (resolvedMemberId is null && data.FamilyMemberName is not null
                && membersByName.TryGetValue(data.FamilyMemberName, out var foundMemberId))
            {
                resolvedMemberId = foundMemberId;
            }

            var createReq = new CreateCalendarEventRequest
            {
                Title = data.Title,
                Description = data.Description,
                EventDate = data.EventDate,
                StartTime = data.StartTime,
                EndTime = data.EndTime,
                FamilyMemberId = resolvedMemberId,
                RecurrenceType = data.RecurrenceType,
                RecurrenceDays = data.RecurrenceDays,
            };

            try
            {
                if (data.Id.HasValue && existingEventIds.Contains(data.Id.Value))
                {
                    await calendarApiClient.UpdateCalendarEventAsync(data.Id.Value,
                        new UpdateCalendarEventRequest
                        {
                            Title = createReq.Title,
                            Description = createReq.Description,
                            EventDate = createReq.EventDate,
                            StartTime = createReq.StartTime,
                            EndTime = createReq.EndTime,
                            FamilyMemberId = createReq.FamilyMemberId,
                            RecurrenceType = createReq.RecurrenceType,
                            RecurrenceDays = createReq.RecurrenceDays,
                        }, cancellationToken);
                    updated++;
                }
                else
                {
                    await calendarApiClient.CreateCalendarEventAsync(createReq, cancellationToken);
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
