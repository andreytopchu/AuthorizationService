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
            .ForMember(x => x.Clients, expression => expression.MapFrom(x =>
                x.ClientIds.Select(id => new ClientPolicy {ClientId = id, PolicyName = string.Concat(new[] {id, "_", x.Name})}).ToArray()));

        CreateMap<IUpdatePolicyCommand, Policy>(MemberList.Source)
            .ForMember(x => x.Clients, expression => expression.MapFrom(x =>
                x.ClientIds.Select(id => new ClientPolicy {ClientId = id, PolicyName = string.Concat(new[] {id, "_", x.Name})}).ToArray()));

        CreateMap<Policy, PolicyInfo>()
            .ForMember(x => x.ClientPolicyInfos, expression => expression.MapFrom(x => x.Clients));

        CreateMap<ClientPolicy, ClientPolicyInfo>();

        CreateMap<Policy, PolicyDto>();
    }
}