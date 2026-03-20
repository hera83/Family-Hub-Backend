using System.ComponentModel.DataAnnotations;

namespace FamilyHub.Adm.Configuration;

public sealed class FamilyHubApiOptions
{
    public const string SectionName = "FamilyHubApi";

    [Required]
    [Url]
    public string BaseUrl { get; init; } = string.Empty;

    [Required]
    public string ApiKeyHeaderName { get; init; } = "x-api-key";

    [Required]
    public string ApiKey { get; init; } = string.Empty;
}
