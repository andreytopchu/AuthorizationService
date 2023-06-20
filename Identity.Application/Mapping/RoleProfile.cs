using AutoMapper;
using Identity.Application.Abstractions.Models.Command.Role;
using Identity.Application.Abstractions.Models.Query.Role;
using Identity.Application.Abstractions.Models.Query.User;
using Identity.Domain.Entities;

namespace Identity.Application.Mapping;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<IAddRoleCommand, Role>(MemberList.Source)
            .ForSourceMember(x => x.PolicyIds, expression => expression.DoNotValidate());

        CreateMap<IUpdateRoleCommand, Role>(MemberList.Source)
            .ForSourceMember(x => x.PolicyIds, expression => expression.DoNotValidate());

        CreateMap<Role, RoleInfo>();

        CreateMap<Role, RoleDto>();
    }
}