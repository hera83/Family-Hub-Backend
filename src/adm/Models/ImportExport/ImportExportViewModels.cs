using System.ComponentModel.DataAnnotations;

namespace FamilyHub.Adm.Models.ImportExport;

public sealed class ImportPreviewRowViewModel
{
    [Display(Name = "Raekke")]
    public int RowNumber { get; init; }

    [Display(Name = "Kolonnevaerdier")]
    public IReadOnlyDictionary<string, string?> Values { get; init; } = new Dictionary<string, string?>();

    [Display(Name = "Gyldig")]
    public bool IsValid { get; init; } = true;

    [Display(Name = "Bemaerkning")]
    public string? Note { get; init; }
}

public sealed class ImportResultViewModel
{
    [Display(Name = "Antal laeste raekker")]
    public int RowsRead { get; init; }

    [Display(Name = "Antal oprettede")]
    public int CreatedCount { get; init; }

    [Display(Name = "Antal opdaterede")]
    public int UpdatedCount { get; init; }

    [Display(Name = "Antal fejl")]
    public int ErrorCount { get; init; }

    [Display(Name = "Raekkefejl")]
    public IReadOnlyList<ImportRowErrorViewModel> RowErrors { get; init; } = [];
}

public sealed class ImportRowErrorViewModel
{
    [Display(Name = "Raekke")]
    public int RowNumber { get; init; }

    [Display(Name = "Fejlbesked")]
    public string ErrorMessage { get; init; } = string.Empty;
}
