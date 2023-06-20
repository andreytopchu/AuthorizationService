using AutoMapper;
using Identity.Application.Abstractions.Models.Command.Client;
using Identity.Application.Abstractions.Models.Query.Client;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Client = IdentityServer4.EntityFramework.Entities.Client;
using ClientClaim = IdentityServer4.EntityFramework.Entities.ClientClaim;

namespace Identity.Application.Mapping;

public class ClientProfile : Profile
{
    public ClientProfile()
    {
        CreateMap<IAddClientCommand, Client>(MemberList.Source)
            .ForMember(x => x.ClientSecrets,
                expression => expression.MapFrom(x => x.ApiSecrets.Select(s => new ClientSecret {Value = s}).ToArray()))
            .ForMember(x => x.AccessTokenType, expression => expression.MapFrom(x => (int) x.AccessTokenType))
            .ForMember(x => x.AllowedGrantTypes,
                expression => expression.MapFrom(x => x.AllowedGrantTypes.Select(g => new ClientGrantType {GrantType = g}).ToArray()))
            .ForMember(x => x.AllowedScopes,
                expression => expression.MapFrom(x => x.AllowedScopes.Select(sc => new ClientScope {Scope = sc}).ToArray()))
            .ForMember(x => x.Claims, expression => expression.MapFrom(x => x.UserClaims.Select(uc => new ClientClaim {Value = uc})))
            .ForMember(x => x.RefreshTokenUsage, expression => expression.MapFrom(x => (int) x.RefreshTokenUsage))
            .ForMember(x => x.RefreshTokenExpiration, expression => expression.MapFrom(x => (int) x.RefreshTokenExpiration))
            .ForMember(x => x.Enabled, expression => expression.MapFrom(x => x.IsEnabled))
            .ForSourceMember(x => x.ApiSecrets, expression => expression.DoNotValidate())
            .ForSourceMember(x => x.UserClaims, expression => expression.DoNotValidate());

        CreateMap<IUpdateClientCommand, Client>(MemberList.Source)
            .ForMember(x => x.AccessTokenType, expression => expression.MapFrom(x => (int) x.AccessTokenType))
            .ForMember(x => x.RefreshTokenUsage, expression => expression.MapFrom(x => (int) x.RefreshTokenUsage))
            .ForMember(x => x.RefreshTokenExpiration, expression => expression.MapFrom(x => (int) x.RefreshTokenExpiration))
            .ForMember(x => x.Enabled, expression => expression.MapFrom(x => x.IsEnabled))
            .ForMember(x => x.AllowedGrantTypes, expression => expression.Ignore())
            .ForMember(x => x.AllowedScopes, expression => expression.Ignore())
            .ForSourceMember(x => x.ApiSecrets, expression => expression.DoNotValidate())
            .ForSourceMember(x => x.UserClaims, expression => expression.DoNotValidate());

        CreateMap<Client, ClientInfo>()
            .ForMember(x => x.AccessTokenType, expression => expression.MapFrom(x => (AccessTokenType) x.AccessTokenType))
            .ForMember(x => x.RefreshTokenUsage, expression => expression.MapFrom(x => (TokenUsage) x.RefreshTokenUsage))
            .ForMember(x => x.RefreshTokenExpiration, expression => expression.MapFrom(x => (TokenExpiration) x.RefreshTokenExpiration))
            .ForMember(x => x.ApiSecrets, expression => expression.MapFrom(x => x.ClientSecrets.Select(s => s.Value).ToArray()))
            .ForMember(x => x.AllowedGrantTypes, expression => expression.MapFrom(x => x.AllowedGrantTypes.Select(s => s.GrantType).ToArray()))
            .ForMember(x => x.AllowedScopes, expression => expression.MapFrom(x => x.AllowedScopes.Select(s => s.Scope).ToArray()))
            .ForMember(x => x.IsEnabled, expression => expression.MapFrom(x => x.Enabled));
    }
}