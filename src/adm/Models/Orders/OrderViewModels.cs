using System.ComponentModel.DataAnnotations;

namespace FamilyHub.Adm.Models.Orders;

public sealed class OrderListItemViewModel
{
    public Guid Id { get; init; }

    [Display(Name = "Status")]
    public string Status { get; init; } = string.Empty;

    [Display(Name = "Antal varer")]
    public int TotalItems { get; init; }

    [Display(Name = "Totalpris")]
    public decimal? TotalPrice { get; init; }

    [Display(Name = "Har PDF")]
    public bool HasPdf { get; init; }

    [Display(Name = "Oprettet")]
    public DateTime CreatedAtUtc { get; init; }

    [Display(Name = "Opdateret")]
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed class OrderLineViewModel
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }

    [Display(Name = "Produkt")]
    public string ProductName { get; init; } = string.Empty;

    [Display(Name = "Kategori")]
    public string? CategoryName { get; init; }

    [Display(Name = "Antal")]
    public decimal? Quantity { get; init; }

    [Display(Name = "Enhed")]
    public string? Unit { get; init; }

    [Display(Name = "Pris")]
    public decimal? Price { get; init; }

    [Display(Name = "Størrelse")]
    public string? SizeLabel { get; init; }

    [Display(Name = "Oprettet")]
    public DateTime CreatedAtUtc { get; init; }

    [Display(Name = "Opdateret")]
    public DateTime? UpdatedAtUtc { get; init; }
}

public sealed class OrderDetailsViewModel
{
    public Guid Id { get; init; }

    [Display(Name = "Status")]
    public string Status { get; init; } = string.Empty;

    [Display(Name = "Antal varer")]
    public int TotalItems { get; init; }

    [Display(Name = "Totalpris")]
    public decimal? TotalPrice { get; init; }

    [Display(Name = "Noter")]
    public string? Notes { get; init; }

    [Display(Name = "Har PDF")]
    public bool HasPdf { get; init; }

    [Display(Name = "Oprettet")]
    public DateTime CreatedAtUtc { get; init; }

    [Display(Name = "Opdateret")]
    public DateTime? UpdatedAtUtc { get; init; }

    public IReadOnlyList<OrderLineViewModel> Lines { get; init; } = [];
}

public sealed class OrderPdfViewModel
{
    public Guid OrderId { get; init; }

    [Display(Name = "Har PDF")]
    public bool HasPdf { get; init; }

    [Display(Name = "Indholdstype")]
    public string ContentType { get; init; } = string.Empty;

    [Display(Name = "Filnavn")]
    public string FileName { get; init; } = string.Empty;

    [Display(Name = "PdfData")]
    public string? PdfData { get; init; }
}
