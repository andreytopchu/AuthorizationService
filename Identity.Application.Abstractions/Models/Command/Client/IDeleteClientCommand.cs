using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.Abstractions.Models.Command.Client;

public interface IDeleteClientCommand : IUseCaseArg
{
    public string ClientId { get; init; }
}