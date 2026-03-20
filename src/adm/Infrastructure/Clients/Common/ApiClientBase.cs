using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FamilyHub.Adm.Infrastructure.Clients.Common;

public abstract class ApiClientBase(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient = httpClient;

    protected Task<TResponse> GetAsync<TResponse>(string uri, CancellationToken cancellationToken = default)
        => SendAsync<TResponse>(HttpMethod.Get, uri, null, cancellationToken);

    protected Task<TResponse> PostAsync<TRequest, TResponse>(string uri, TRequest payload, CancellationToken cancellationToken = default)
        => SendAsync<TResponse>(HttpMethod.Post, uri, payload, cancellationToken);

    protected Task<TResponse> PutAsync<TRequest, TResponse>(string uri, TRequest payload, CancellationToken cancellationToken = default)
        => SendAsync<TResponse>(HttpMethod.Put, uri, payload, cancellationToken);

    protected Task DeleteAsync(string uri, CancellationToken cancellationToken = default)
        => SendAsync(HttpMethod.Delete, uri, null, cancellationToken);

    private async Task<TResponse> SendAsync<TResponse>(HttpMethod method, string uri, object? payload, CancellationToken cancellationToken)
    {
        using var response = await SendAsync(method, uri, payload, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken);
        if (result is null)
        {
            throw new ApiClientException(response.StatusCode, "API returnerede tom data.");
        }

        return result;
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string uri, object? payload, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, uri);
        if (payload is not null)
        {
            request.Content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json");
        }

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return response;
        }

        var userMessage = await BuildUserMessageAsync(response, cancellationToken);
        var technicalMessage = $"API call failed with status {(int)response.StatusCode} ({response.StatusCode}) for {method} {uri}.";

        response.Dispose();
        throw new ApiClientException(response.StatusCode, userMessage, technicalMessage);
    }

    private static async Task<string> BuildUserMessageAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        const string fallback = "api svarede med en fejl. Prøv igen om lidt.";

        if (response.Content.Headers.ContentType?.MediaType?.Contains("json", StringComparison.OrdinalIgnoreCase) != true)
        {
            return fallback;
        }

        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(JsonOptions, cancellationToken);
            if (!string.IsNullOrWhiteSpace(problem?.Detail))
            {
                return problem.Detail;
            }

            if (!string.IsNullOrWhiteSpace(problem?.Title))
            {
                return problem.Title;
            }

            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => "Adgang nægtet til api. Tjek API key.",
                HttpStatusCode.Forbidden => "Adgang forbudt til api.",
                HttpStatusCode.NotFound => "Den ønskede ressource blev ikke fundet i api.",
                _ => fallback
            };
        }
        catch
        {
            return fallback;
        }
    }

    protected static string WithQueryString(string path, IReadOnlyDictionary<string, string?> query)
    {
        var parts = query
            .Where(pair => !string.IsNullOrWhiteSpace(pair.Value))
            .Select(pair => $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value!)}")
            .ToArray();

        if (parts.Length == 0)
        {
            return path;
        }

        return $"{path}?{string.Join("&", parts)}";
    }
}
