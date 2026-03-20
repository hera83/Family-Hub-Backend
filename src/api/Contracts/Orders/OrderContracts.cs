namespace FamilyHub.Api.Contracts.Orders;

// ─── Query ───────────────────────────────────────────────────────────────────

public sealed class OrderListQueryRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 25;

    /// <summary>Filtrér på status (Created, Confirmed, Completed, Cancelled).</summary>
    public string? Status { get; init; }
}

// ─── List ─────────────────────────────────────────────────────────────────────

public sealed record OrderListItemDto(
    Guid Id,
    string Status,
    int TotalItems,
    decimal? TotalPrice,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    bool HasPdf
);

// ─── Details ─────────────────────────────────────────────────────────────────

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
