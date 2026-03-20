using FamilyHub.Adm.Models.Api.Common;

namespace FamilyHub.Adm.Models.Api.Orders;

public sealed class OrderListQueryRequest
{
    public string? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 25;
}

public sealed record OrderListItemDto(
    Guid Id,
    string Status,
    int TotalItems,
    decimal? TotalPrice,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    bool HasPdf
);

public sealed record OrderLineDto(
    Guid Id,
    Guid OrderId,
    string ProductName,
    string? CategoryName,
    decimal? Quantity,
    string? Unit,
    decimal? Price,
    string? SizeLabel,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);

public sealed record OrderDetailsDto(
    Guid Id,
    string Status,
    int TotalItems,
    decimal? TotalPrice,
    string? Notes,
    string? PdfData,
    bool HasPdf,
    IReadOnlyList<OrderLineDto> Lines,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);

public sealed record OrderPdfDto(
    Guid OrderId,
    bool HasPdf,
    string? PdfData,
    string ContentType,
    string FileName
);
