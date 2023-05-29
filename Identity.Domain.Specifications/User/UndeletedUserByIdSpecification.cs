using Dex.Specifications;

namespace Identity.Domain.Specifications.User
{
    public class UndeletedUserByIdSpecification : AndSpecification<Domain.Entities.User>
    {
        public UndeletedUserByIdSpecification(Guid userId) : base(new Specification<Entities.User>(db => db.Id == userId),
            new UndeleteEntitySpecification<Entities.User>())
        {
        }
    }
}