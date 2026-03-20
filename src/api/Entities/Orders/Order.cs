namespace FamilyHub.Api.Entities.Orders;

public class Order : BaseEntity
{
    public string Status { get; set; } = OrderStatus.Created;
    public int TotalItems { get; set; }
    public decimal? TotalPrice { get; set; }
    public string? Notes { get; set; }

    /// <summary>
    /// Valgfri PDF gemt som base64-streng eller data-URL direkte i databasen (v1 simpel tilgang).
    /// </summary>
    public string? PdfData { get; set; }

    public ICollection<OrderLine> Lines { get; set; } = [];
}
