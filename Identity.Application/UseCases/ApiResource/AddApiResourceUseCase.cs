using Identity.Application.Abstractions.Models.Command.ApiResource;
using Identity.Application.Abstractions.Models.Query.ApiResource;
using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.UseCases.ApiResource;

public class AddApiResourceUseCase : IUseCase<IAddApiResourceCommand, ApiResourceInfo>
{
    public async Task<ApiResourceInfo> Process(IAddApiResourceCommand arg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}