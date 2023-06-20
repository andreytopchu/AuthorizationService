using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Identity.Abstractions;
using Identity.Application.Abstractions.Repositories.Policy;
using Identity.Domain.Specifications;
using Identity.Domain.Specifications.Policy;
using Microsoft.EntityFrameworkCore;

namespace Identity.Dal.Repository.Policy;

public class PolicyReadRepository : GenericReadRepository<Domain.Entities.Policy, Guid>, IPolicyReadRepository
{
    private readonly IMapper _mapper;

    public PolicyReadRepository(IReadDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
        _mapper = mapper;
    }

    public async Task<bool> IsPolicyExistAsync(Guid id, CancellationToken cancellationToken)
    {
        var policy = await FirstOrDefaultAsync(new EntityByKeySpecification<Domain.Entities.Policy, Guid>(id), cancellationToken);

        return policy is not null;
    }

    public async Task<TInfo[]> GetPolicies<TInfo>(Guid[] policyIds, CancellationToken cancellationToken)
    {
        return await QueryBy(new PolicyByIdsSpecification(policyIds))
            .ProjectTo<TInfo>(_mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken);
    }
}