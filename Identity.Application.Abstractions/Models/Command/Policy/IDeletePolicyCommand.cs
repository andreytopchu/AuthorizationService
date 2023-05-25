using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.Abstractions.Models.Command.Policy;

public interface IDeletePolicyCommand : IUseCaseArg
{
    public Guid PolicyId { get; init; }
}