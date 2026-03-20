using FamilyHub.Adm.Configuration;
using Microsoft.Extensions.Options;

namespace FamilyHub.Adm.Infrastructure.Clients;

public sealed class ApiKeyHeaderHandler(IOptions<FamilyHubApiOptions> apiOptions) : DelegatingHandler
{
    private readonly FamilyHubApiOptions _apiOptions = apiOptions.Value;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!request.Headers.Contains(_apiOptions.ApiKeyHeaderName))
        {
            request.Headers.Add(_apiOptions.ApiKeyHeaderName, _apiOptions.ApiKey);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
