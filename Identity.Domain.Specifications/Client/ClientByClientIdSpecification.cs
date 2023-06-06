using Dex.Specifications;

namespace Identity.Domain.Specifications.Client;

public class ClientByClientIdSpecification : Specification<IdentityServer4.EntityFramework.Entities.Client>
{
    public ClientByClientIdSpecification(string clientId) : base(x => x.ClientId == clientId)
    {
    }
}