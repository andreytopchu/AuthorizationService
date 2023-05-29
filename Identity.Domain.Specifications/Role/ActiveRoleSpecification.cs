using Dex.Specifications;

namespace Identity.Domain.Specifications.Role;

public class ActiveRoleSpecification : Specification<Entities.Role>
{
    public ActiveRoleSpecification() : base(db => !db.DeletedUtc.HasValue)
    {
    }

    public ActiveRoleSpecification(Guid roleId) : base(db => db.Id == roleId && !db.DeletedUtc.HasValue)
    {
    }
}