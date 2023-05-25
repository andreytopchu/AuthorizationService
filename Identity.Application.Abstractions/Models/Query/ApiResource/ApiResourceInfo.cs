namespace Identity.Application.Abstractions.Models.Query.ApiResource;

public class ApiResourceInfo
{
    public string Name { get; init; }
    public string DisplayName { get; init; }
    public List<string> Scopes { get; init; }
    public List<string> UserClaims { get; init; }
    public List<string> ApiSecrets { get; init; }
    public bool IsEnabled { get; init; }
}