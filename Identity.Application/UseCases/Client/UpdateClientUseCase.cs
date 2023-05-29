using Identity.Application.Abstractions.Models.Command.Client;
using Identity.Application.Abstractions.Models.Query.Client;
using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.UseCases.Client;

public class UpdateClientUseCase : IUseCase<IUpdateClientCommand, ClientInfo>
{
    public async Task<ClientInfo> Process(IUpdateClientCommand arg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}