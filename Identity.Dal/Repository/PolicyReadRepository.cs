using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Identity.Abstractions;
using Identity.Application.Abstractions.Repositories.Policy;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications;
using Identity.Domain.Specifications.Policy;
using Microsoft.EntityFrameworkCore;

namespace Identity.Dal.Repository;

public class PolicyReadRepository : GenericReadRepository<Policy, Guid>, IPolicyReadRepository
{
    private readonly IMapper _mapper;

    public PolicyReadRepository(IReadDbContext dbContext, IMapper mapper) : base(dbContext)
    {
        _mapper = mapper;
    }

    public async Task<bool> IsPolicyExistAsync(Guid id, CancellationToken cancellationToken)
    {
        var policy = await FirstOrDefaultAsync(new EntityByKeySpecification<Policy, Guid>(id), cancellationToken);

        return policy is not null;
    }

    public async Task<TInfo> GetPolicyByIdAsync<TInfo>(Guid id, CancellationToken cancellationToken)
    {
        var policyInfo = await QueryBy(new EntityByKeySpecification<Policy, Guid>(id))
            .ProjectTo<TInfo>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (policyInfo == null)
            throw new PolicyNotFoundException(id);

        return policyInfo;
    }

    public async Task<TInfo[]> GetPolicies<TInfo>(Guid[] policyIds, CancellationToken cancellationToken)
    {
        return await QueryBy(new PolicyByIdsSpecification(policyIds))
            .ProjectTo<TInfo>(_mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken);
    }
}