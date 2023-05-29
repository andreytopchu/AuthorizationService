using Dex.Specifications;

namespace Identity.Domain.Specifications.User
{
    public class ActiveUserByRoleIdSpecification : AndSpecification<Entities.User>
    {
        public ActiveUserByRoleIdSpecification(Guid roleId) : base(new Specification<Entities.User>(db => db.RoleId == roleId),
            new ActiveUserSpecification())
        {
        }
    }
}