using FamilyHub.Api.Contracts.Orders;
using FamilyHub.Api.Entities.Orders;

namespace FamilyHub.Api.Features.Orders;

internal static class OrderMappings
{
    internal static OrderListItemDto ToListItemDto(this Order order) => new(
        order.Id,
        order.Status,
        order.TotalItems,
        order.TotalPrice,
        order.CreatedAtUtc,
        order.UpdatedAtUtc,
        !string.IsNullOrWhiteSpace(order.PdfData));

    internal static OrderLineDto ToLineDto(this OrderLine line) => new(
        line.Id,
        line.OrderId,
        line.ProductName,
        line.CategoryName,
        line.Quantity,
        line.Unit,
        line.Price,
        line.SizeLabel,
        line.CreatedAtUtc,
        line.UpdatedAtUtc);

    internal static OrderDetailsDto ToDetailsDto(this Order order) => new(
        order.Id,
        order.Status,
        order.TotalItems,
        order.TotalPrice,
        order.Notes,
        order.PdfData,
        !string.IsNullOrWhiteSpace(order.PdfData),
        order.Lines.Select(l => l.ToLineDto()).ToList(),
        order.CreatedAtUtc,
        order.UpdatedAtUtc);

    internal static OrderPdfDto ToPdfDto(this Order order)
    {
        var hasPdf = !string.IsNullOrWhiteSpace(order.PdfData);

        // V1: PDF lagres som tekst (base64/data-url) direkte på ordren for enkelhed.
        // Kan senere flyttes til filstorage/blob storage uden at ændre OrderLine historikmodellen.
        return new OrderPdfDto(
            order.Id,
            hasPdf,
            hasPdf ? order.PdfData : null,
            hasPdf ? "application/pdf" : string.Empty,
            hasPdf ? $"ordre-{order.Id}.pdf" : string.Empty);
    }
}
