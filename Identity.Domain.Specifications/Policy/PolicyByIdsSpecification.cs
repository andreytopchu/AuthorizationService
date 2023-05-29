using Dex.Specifications;

namespace Identity.Domain.Specifications.Policy;

public class PolicyByIdsSpecification : AndSpecification<Entities.Policy>
{
    public PolicyByIdsSpecification(Guid[] policyIds) : base(new Specification<Entities.Policy>(db => policyIds.Contains(db.Id)),
        new UndeleteEntitySpecification<Entities.Policy>())
    {
    }
}