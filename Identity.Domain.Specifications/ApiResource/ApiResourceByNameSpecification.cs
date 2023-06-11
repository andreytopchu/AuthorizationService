using Dex.Specifications;

namespace Identity.Domain.Specifications.ApiResource;

public class ApiResourceByNameSpecification : Specification<IdentityServer4.EntityFramework.Entities.ApiResource>
{
    public ApiResourceByNameSpecification(string name) : base(x => x.Name == name)
    {
    }
}