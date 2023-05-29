using Dex.Specifications;

namespace Identity.Domain.Specifications.Role;

public class ActiveRoleSpecification : AndSpecification<Entities.Role>
{
    public ActiveRoleSpecification() : base(new Specification<Entities.Role>(db => !db.DeletedUtc.HasValue))
    {
    }

    public ActiveRoleSpecification(Guid roleId) : base(new Specification<Entities.Role>(db => db.Id == roleId),
        new UndeleteEntitySpecification<Entities.Role>())
    {
    }
}