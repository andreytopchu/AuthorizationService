using Identity.Abstractions.Repository;

namespace Identity.Application.Abstractions.Repositories.Policy;

public interface IPolicyReadRepository : IReadRepository<Domain.Entities.Policy, Guid>
{
    Task<bool> IsPolicyExistAsync(Guid id, CancellationToken cancellationToken);
    Task<TInfo> GetPolicyByIdAsync<TInfo>(Guid policyId, CancellationToken cancellationToken);
    Task<TInfo[]> GetPolicies<TInfo>(Guid[] policyIds, CancellationToken cancellationToken);
}