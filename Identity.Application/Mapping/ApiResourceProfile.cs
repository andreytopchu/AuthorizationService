using AutoMapper;
using Identity.Application.Abstractions.Models.Command.ApiResource;
using Identity.Application.Abstractions.Models.Query.ApiResource;
using IdentityModel;
using IdentityServer4.EntityFramework.Entities;

namespace Identity.Application.Mapping;

public class ApiResourceProfile : Profile
{
    public ApiResourceProfile()
    {
        CreateMap<IAddApiResourceCommand, ApiResource>(MemberList.Source)
            .ForMember(x => x.Scopes, expression => expression.MapFrom(x => x.Scopes.Select(s => new ApiResourceScope {Scope = s}).ToArray()))
            .ForMember(x => x.Secrets, expression => expression.MapFrom(x => x.ApiSecrets.Select(s => new ApiResourceSecret {Value = s.ToSha256()}).ToArray()))
            .ForMember(x => x.UserClaims, expression => expression.MapFrom(x => x.UserClaims.Select(uc => new ApiResourceClaim {Type = uc}).ToArray()))
            .ForMember(x => x.Enabled, expression => expression.MapFrom(x => x.IsEnabled))
            .ForSourceMember(x => x.ApiSecrets, expression => expression.DoNotValidate());

        CreateMap<IUpdateApiResourceCommand, ApiResource>(MemberList.Source)
            .ForMember(x => x.Scopes, expression => expression.Ignore())
            .ForMember(x => x.Secrets, expression => expression.Ignore())
            .ForMember(x => x.UserClaims, expression => expression.Ignore())
            .ForMember(x => x.Enabled, expression => expression.MapFrom(x => x.IsEnabled))
            .ForSourceMember(x => x.ApiSecrets, expression => expression.DoNotValidate());


        CreateMap<ApiResource, ApiResourceInfo>()
            .ForMember(x => x.Scopes, expression => expression.MapFrom(x => x.Scopes.Select(s => s.Scope).ToArray()))
            .ForMember(x => x.ApiSecrets, expression => expression.MapFrom(x => x.Secrets.Select(s => s.Value).ToArray()))
            .ForMember(x => x.UserClaims, expression => expression.MapFrom(x => x.UserClaims.Select(s => s.Type).ToArray()))
            .ForMember(x => x.IsEnabled, expression => expression.MapFrom(x => x.Enabled));
    }
}