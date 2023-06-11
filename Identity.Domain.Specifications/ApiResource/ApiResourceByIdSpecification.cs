using Dex.Specifications;

namespace Identity.Domain.Specifications.ApiResource;

public class ApiResourceByIdSpecification : Specification<IdentityServer4.EntityFramework.Entities.ApiResource>
{
    public ApiResourceByIdSpecification(int id) : base(x => x.Id == id)
    {
    }
}