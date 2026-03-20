namespace FamilyHub.Api.Entities.Orders;

/// <summary>
/// Historisk snapshot af én ordrelinje på det tidspunkt ordren blev oprettet.
/// Ingen foreign key til Product – værdier kopieres direkte ind så
/// ændringer i produktkataloget ikke påvirker ordrehistorikken.
/// </summary>
public class OrderLine : BaseEntity
{
    public Guid OrderId { get; set; }

    /// <summary>Produktnavn snapshot – kopieret fra kataloget ved ordretidspunktet.</summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>Kategorinavn snapshot.</summary>
    public string? CategoryName { get; set; }

    public decimal? Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal? Price { get; set; }
    public string? SizeLabel { get; set; }

    public Order Order { get; set; } = null!;
}
