using System;
using System.Collections.Generic;
using IdentityServer4.Models;

namespace Identity.Models.Requests.Client;

public class BaseClientRequest
{
    public string ClientId { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;
    public ICollection<string> ApiSecrets { get; init; } = Array.Empty<string>();
    public ICollection<string> AllowedScopes { get; init; } = Array.Empty<string>();
    public ICollection<string> UserClaims { get; init; } = Array.Empty<string>();
    public AccessTokenType AccessTokenType { get; init; }
    public ICollection<string> AllowedGrantTypes { get; init; } = Array.Empty<string>();
    public bool AllowOfflineAccess { get; init; }
    public bool UpdateAccessTokenClaimsOnRefresh { get; init; }
    public TokenUsage RefreshTokenUsage { get; init; }
    public TokenExpiration RefreshTokenExpiration { get; init; }
    public int AccessTokenLifetime { get; init; }
    public int AbsoluteRefreshTokenLifetime { get; init; }
    public int SlidingRefreshTokenLifetime { get; init; }
    public bool IsEnabled { get; init; }
}