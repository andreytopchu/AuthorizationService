using Identity.Application.Abstractions.UseCases;
using IdentityServer4.Models;

namespace Identity.Application.Abstractions.Models.Command.Client;

public interface IUpdateClientCommand : IUseCaseArg
{
    public string ClientId { get; init; }
    public string ClientName { get; init; }
    public ICollection<string> ApiSecrets { get; init; }
    public ICollection<string> AllowedScopes { get; init; }
    public ICollection<string> UserClaims { get; init; }
    public AccessTokenType AccessTokenType { get; init; }
    public ICollection<string> AllowedGrantTypes { get; init; }
    public bool AllowOfflineAccess { get; init; }
    public bool UpdateAccessTokenClaimsOnRefresh { get; init; }
    public TokenUsage RefreshTokenUsage { get; init; }
    public TokenExpiration RefreshTokenExpiration { get; init; }
    public int AccessTokenLifetime { get; init; }
    public int AbsoluteRefreshTokenLifetime { get; init; }
    public int SlidingRefreshTokenLifetime { get; init; }
    public bool IsEnabled { get; init; }
}