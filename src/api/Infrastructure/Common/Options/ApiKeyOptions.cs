namespace FamilyHub.Api.Infrastructure.Common.Options;

/// <summary>
/// Konfigurationsindstillinger for API key-validering.
/// Bindes fra appsettings.json sektion "ApiKey".
/// </summary>
public class ApiKeyOptions
{
    /// <summary>Header-navn til API-nøgle (standard: "x-api-key").</summary>
    public string HeaderName { get; set; } = "x-api-key";

    /// <summary>Den gyldige API-nøgle til validering.</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Indikerer om API-nøgle-validering er aktiveret.</summary>
    public bool Enabled { get; set; } = true;
}
