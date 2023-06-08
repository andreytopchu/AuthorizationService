using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.Abstractions.Models.Command.ApiResource;

public interface IDeleteApiResourceCommand : IUseCaseArg
{
    public int ApiResourceId { get; init; }
}