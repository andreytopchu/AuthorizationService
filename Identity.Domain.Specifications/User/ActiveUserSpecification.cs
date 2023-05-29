using Dex.Specifications;

namespace Identity.Domain.Specifications.User;

public class ActiveUserSpecification : AndSpecification<Entities.User>
{
    public ActiveUserSpecification()
        : base(new Specification<Entities.User>(db => db.EmailConfirmed.HasValue), new UndeleteEntitySpecification<Entities.User>())
    {
    }
}