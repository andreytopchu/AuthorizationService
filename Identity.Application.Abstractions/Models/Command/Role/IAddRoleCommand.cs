using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.Abstractions.Models.Command.Role;

/// <summary>
/// Интерфейс команды добавления роли
/// </summary>
public interface IAddRoleCommand : IUseCaseArg
{
    /// <summary>
    /// название роли
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// права доступа роли
    /// </summary>
    public Guid[] PolicyIds { get; init; }

    string? Description { get; init; }
}