using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.Abstractions.Models.Command.ApiResource;

public interface IUpdateApiResourceCommand : IUseCaseArg
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string DisplayName { get; init; }
    public ICollection<string> Scopes { get; init; }
    public ICollection<string> UserClaims { get; init; }
    public ICollection<string> ApiSecrets { get; init; }
    public bool IsEnabled { get; init; }
}