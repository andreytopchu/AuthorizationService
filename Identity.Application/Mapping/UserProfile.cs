using AutoMapper;
using Identity.Application.Abstractions.Models.Query.User;
using Identity.Domain.Entities;

namespace Identity.Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, Abstractions.Models.Authorization.User>()
            .ForMember(x => x.Subject, expression => expression.MapFrom(x => x.Id.ToString()))
            .ForMember(x => x.Name, expression => expression.MapFrom(x => x.GetFullName()))
            .ForMember(x => x.IsActive, expression => expression.MapFrom(x => !x.DeletedUtc.HasValue && x.EmailConfirmed.HasValue))
            .ForMember(x => x.Claims, expression => expression.Ignore());

        CreateMap<User, UserInfo>(MemberList.Destination)
            .ForMember(x => x.IsActive, expression => expression.MapFrom(x => !x.DeletedUtc.HasValue && x.EmailConfirmed.HasValue));
    }
}