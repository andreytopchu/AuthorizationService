using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.Abstractions.Models.Command.Policy;

/// <summary>
/// Интерфейс команды добавления права доступа
/// </summary>
public interface IAddPolicyCommand : IUseCaseArg
{
    public string Name { get; init; }
    public string[] ResourceNames { get; init; }
}