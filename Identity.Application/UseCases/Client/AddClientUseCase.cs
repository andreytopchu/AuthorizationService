using Identity.Application.Abstractions.Models.Command.Client;
using Identity.Application.Abstractions.Models.Query.Client;
using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.UseCases.Client;

public class AddClientUseCase : IUseCase<IAddClientCommand, ClientInfo>
{
    public async Task<ClientInfo> Process(IAddClientCommand arg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}