using System;
using AutoMapper;
using Identity.Abstractions;
using Identity.Application.Abstractions.Repositories.User;

namespace Identity.Dal.Repository.User;

public class UserWriteRepository : GenericWriteRepository<Domain.Entities.User, Guid, IUserReadRepository>, IUserWriteRepository
{
    public UserWriteRepository(IWriteDbContext writeDbContext, IMapper mapper)
        : base(writeDbContext, new UserReadRepository(writeDbContext, mapper))
    {
    }
}