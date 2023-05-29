using Dex.Entity;

namespace Identity.Domain.Entities;

public class Client : IdentityServer4.EntityFramework.Entities.Client, IEntity<int>
{
    public ICollection<ClientPolicy> Policies { get; set; } = Array.Empty<ClientPolicy>();
}