using Dex.Specifications;

namespace Identity.Domain.Specifications.User;

public class ActiveUserByIdSpecification : AndSpecification<Entities.User>
{
    public ActiveUserByIdSpecification(Guid userId)
        : base(new Specification<Entities.User>(db => db.Id == userId), new ActiveUserSpecification())
    {
    }
}