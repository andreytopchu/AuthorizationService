using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.Abstractions.Models.Command.Client;

public interface IAddClientCommand : IUseCaseArg
{
    public string Name { get; init; }
    public string DisplayName { get; init; }
    public List<string> Scopes { get; init; }
    public List<string> UserClaims { get; init; }
    public List<string> ApiSecrets { get; init; }
    public bool IsEnabled { get; init; }
}