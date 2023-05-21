using Identity.Application.Abstractions.Models.Admin;

namespace Identity.Application.Abstractions.Services;

public interface IPolicyService
{
    Task AddPolicy(AddPolicyRequest request, CancellationToken cancellationToken);
    Task UpdatePolicy(UpdatePolicyRequest request, CancellationToken cancellationToken);
    Task DeletePolicy(Guid policyId, CancellationToken cancellationToken);
}