using Dex.Specifications;

namespace Identity.Domain.Specifications.User;

public class ActiveUserByPolicyIdSpecification : AndSpecification<Entities.User>
{
    public ActiveUserByPolicyIdSpecification(Guid policyId) : base(new Specification<Entities.User>(db => db.Role.Policies.Select(x => x.Id).Contains(policyId)),
        new ActiveUserSpecification())
    {
    }
}