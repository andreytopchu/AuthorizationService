using Dex.Specifications;

namespace Identity.Domain.Specifications.User
{
    public class ActiveUserByEmailSpecification : AndSpecification<Entities.User>
    {
        public ActiveUserByEmailSpecification(string userEmail) : base(new Specification<Entities.User>(db => db.Email == userEmail.ToLower()),
            new ActiveUserSpecification())
        {
        }
    }
}