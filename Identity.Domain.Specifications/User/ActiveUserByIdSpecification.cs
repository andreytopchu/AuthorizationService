using Dex.Specifications;

namespace Identity.Domain.Specifications.User;

public class ActiveUserByIdSpecification : AndSpecification<Entities.User>
{
    public ActiveUserByIdSpecification(Guid sub)
        : base(new Specification<Entities.User>(db => db.Id == sub), new ActiveUserSpecification())
    {
    }
}