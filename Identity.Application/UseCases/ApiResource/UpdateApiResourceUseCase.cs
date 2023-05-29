using Identity.Application.Abstractions.Models.Command.ApiResource;
using Identity.Application.Abstractions.Models.Query.ApiResource;
using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.UseCases.ApiResource;

public class UpdateApiResourceUseCase : IUseCase<IUpdateApiResourceCommand, ApiResourceInfo>
{
    public async Task<ApiResourceInfo> Process(IUpdateApiResourceCommand arg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}