using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.Abstractions.Models.Command.Policy;

public interface IUpdatePolicyCommand : IUseCaseArg
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string ClientId { get; init; }
}