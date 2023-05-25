using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.Abstractions.Models.Command.Role;

/// <summary>
/// Интерфейс команды обновления роли
/// </summary>
public interface IUpdateRoleCommand : IUseCaseArg
{
    /// <summary>
    /// Id роли
    /// </summary>
    public Guid Id { get; init; }

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