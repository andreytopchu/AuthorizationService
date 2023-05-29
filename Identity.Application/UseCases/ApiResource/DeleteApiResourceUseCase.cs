using Identity.Application.Abstractions.Models.Command.ApiResource;
using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.UseCases.ApiResource;

public class DeleteApiResourceUseCase : IUseCase<IDeleteApiResourceCommand>
{
    public async Task Process(IDeleteApiResourceCommand arg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}