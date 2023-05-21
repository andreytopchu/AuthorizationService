using IdentityServer4.Models;

namespace Identity.Application.Abstractions.Models.Admin;

public class UpdateClientRequest
{
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