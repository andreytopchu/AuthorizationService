using AutoMapper;
using Identity.Application.Abstractions.Models.Command.Policy;
using Identity.Application.Abstractions.Models.Query.ClientPolicy;
using Identity.Application.Abstractions.Models.Query.Policy;
using Identity.Application.Abstractions.Models.Query.Role;
using Identity.Domain.Entities;

namespace Identity.Application.Mapping;

public class PolicyProfile : Profile
{
    public PolicyProfile()
    {
        CreateMap<IAddPolicyCommand, Policy>(MemberList.Source)
            .ForMember(x => x.ApiResources, expression => expression.MapFrom(x =>
                x.ResourceNames.Select(resourceName => new ApiResourcePolicy
                    {ResourceName = resourceName, PolicyName = string.Concat(new[] {resourceName, "_", x.Name})}).ToArray()))
            .ForSourceMember(x => x.ResourceNames, expression => expression.DoNotValidate());

        CreateMap<IUpdatePolicyCommand, Policy>(MemberList.Source)
            .ForMember(x => x.ApiResources, expression => expression.MapFrom(x =>
                x.ResourceNames.Select(resourceName => new ApiResourcePolicy
                    {ResourceName = resourceName, PolicyName = string.Concat(new[] {resourceName, "_", x.Name})}).ToArray()))
            .ForSourceMember(x => x.ResourceNames, expression => expression.DoNotValidate());

        CreateMap<Policy, PolicyInfo>()
            .ForMember(x => x.ApiResourcePolicyInfos, expression => expression.MapFrom(x => x.ApiResources));

        CreateMap<ApiResourcePolicy, ApiResourcePolicyInfo>();

        CreateMap<Policy, PolicyDto>();
    }
}