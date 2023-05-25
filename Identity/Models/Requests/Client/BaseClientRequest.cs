using System.Collections.Generic;
using IdentityServer4.Models;

namespace Identity.Models.Requests.Client;

public class BaseClientRequest
{
    public string ClientId { get; }
    public string ClientName { get; }
    public AccessTokenType AccessTokenType { get; }
    public ICollection<string> AllowedScopes { get; }
    public bool AllowOfflineAccess { get; }
    public bool UpdateAccessTokenClaimsOnRefresh { get; }
    public TokenUsage RefreshTokenUsage { get; }
    public TokenExpiration RefreshTokenExpiration { get; }
    public int AccessTokenLifetime { get; }
    public int AbsoluteRefreshTokenLifetime { get; }
    public int SlidingRefreshTokenLifetime { get; }
}