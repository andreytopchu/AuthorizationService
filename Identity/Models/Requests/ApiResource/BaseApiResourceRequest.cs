using System;
using System.Collections.Generic;

namespace Identity.Models.Requests.ApiResource;

public class BaseApiResourceRequest
{
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public ICollection<string> Scopes { get; init; } = Array.Empty<string>();
    public ICollection<string> UserClaims { get; init; } = Array.Empty<string>();
    public ICollection<string> ApiSecrets { get; init; } = Array.Empty<string>();
    public bool IsEnabled { get; init; }
}