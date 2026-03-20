namespace FamilyHub.Adm.Services.ImportExport.Models;

public sealed class ImportRowError
{
    public required int RowNumber { get; init; }
    public required string Message { get; init; }
}

public sealed class ImportResult
{
    public required string TypeName { get; init; }
    public required string DisplayName { get; init; }
    public int TotalRead { get; init; }
    public int Created { get; init; }
    public int Updated { get; init; }
    public int Failed { get; init; }
    public IReadOnlyList<ImportRowError> Errors { get; init; } = [];
}
