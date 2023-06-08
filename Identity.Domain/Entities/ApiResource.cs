using Dex.Entity;

namespace Identity.Domain.Entities;

public class ApiResource : IdentityServer4.EntityFramework.Entities.ApiResource, IEntity<int>
{
}