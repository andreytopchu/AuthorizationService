using System;
using AutoMapper;
using Identity.Abstractions;
using Identity.Application.Abstractions.Repositories.Policy;

namespace Identity.Dal.Repository.Policy;

public class PolicyWriteRepository : GenericWriteRepository<Domain.Entities.Policy, Guid, IPolicyReadRepository>, IPolicyWriteRepository
{
    public PolicyWriteRepository(IWriteDbContext writeDbContext, IMapper mapper)
        : base(writeDbContext, new PolicyReadRepository(writeDbContext, mapper))
    {
    }
}