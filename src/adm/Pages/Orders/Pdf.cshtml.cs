using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Infrastructure.Clients.Orders;
using FamilyHub.Adm.Models.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace FamilyHub.Adm.Pages.Orders;

public class PdfModel(IOrdersApiClient ordersApiClient) : PageModel
{
    private readonly IOrdersApiClient _ordersApiClient = ordersApiClient;

    public OrderPdfViewModel? Item { get; private set; }

    public string? LoadErrorMessage { get; private set; }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var pdf = await _ordersApiClient.GetOrderPdfAsync(id, cancellationToken);
            Item = pdf.ToViewModel();
        }
        catch (ApiClientException ex)
        {
            LoadErrorMessage = ex.UserMessage;
        }
    }

    public async Task<IActionResult> OnPostDownloadAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var pdf = await _ordersApiClient.GetOrderPdfAsync(id, cancellationToken);
            if (!pdf.HasPdf || string.IsNullOrWhiteSpace(pdf.PdfData))
            {
                TempData["ErrorMessage"] = "Ordren har ingen PDF-data.";
                return RedirectToPage(new { id });
            }

            if (TryExtractPdfBytes(pdf.PdfData, out var pdfBytes))
            {
                var fileName = string.IsNullOrWhiteSpace(pdf.FileName)
                    ? $"ordre-{pdf.OrderId}.pdf"
                    : Path.ChangeExtension(pdf.FileName, ".pdf");

                return File(pdfBytes, "application/pdf", fileName);
            }

            // Fallback i v1: hvis data ikke kan tolkes som PDF, downloades rå tekst for inspektion.
            var textFileName = string.IsNullOrWhiteSpace(pdf.FileName)
                ? $"ordre-{pdf.OrderId}-pdfdata.txt"
                : Path.ChangeExtension(pdf.FileName, ".txt");

            return File(Encoding.UTF8.GetBytes(pdf.PdfData), "text/plain", textFileName);
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
            return RedirectToPage(new { id });
        }
    }

    private static bool TryExtractPdfBytes(string raw, out byte[] bytes)
    {
        bytes = [];

        if (string.IsNullOrWhiteSpace(raw))
            return false;

        var candidate = raw.Trim();

        // Understøt data-url format: data:application/pdf;base64,....
        var commaIndex = candidate.IndexOf(',');
        if (candidate.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && commaIndex > 0)
        {
            candidate = candidate[(commaIndex + 1)..];
        }

        try
        {
            bytes = Convert.FromBase64String(candidate);
            return bytes.Length > 0;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
