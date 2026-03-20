namespace FamilyHub.Adm.Services.ImportExport;

public sealed record ExcelExportFile(
    string FileName,
    byte[] Content,
    string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
);
