using Identity.Application.Abstractions.Models.Command.Client;
using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.UseCases.Client;

public class DeleteClientUseCase : IUseCase<IDeleteClientCommand>
{
    public async Task Process(IDeleteClientCommand arg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}