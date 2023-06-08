using System;
using AutoMapper;
using Identity.Abstractions;
using Identity.Application.Abstractions.Repositories.Role;

namespace Identity.Dal.Repository.Role;

public class RoleWriteRepository : GenericWriteRepository<Domain.Entities.Role, Guid, IRoleReadRepository>, IRoleWriteRepository
{
    public RoleWriteRepository(IWriteDbContext writeDbContext, IMapper mapper)
        : base(writeDbContext, new RoleReadRepository(writeDbContext, mapper))
    {
    }
}