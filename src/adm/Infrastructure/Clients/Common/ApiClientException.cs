using System.Net;

namespace FamilyHub.Adm.Infrastructure.Clients.Common;

public sealed class ApiClientException : Exception
{
    public ApiClientException(HttpStatusCode statusCode, string userMessage, string? technicalMessage = null)
        : base(technicalMessage ?? userMessage)
    {
        StatusCode = statusCode;
        UserMessage = userMessage;
    }

    public HttpStatusCode StatusCode { get; }

    public string UserMessage { get; }
}
