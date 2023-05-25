using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.Abstractions.Models.Command.Role;

/// <summary>
/// Интерфейс команды удаления роли
/// </summary>
public interface IDeleteRoleCommand : IUseCaseArg
{
    /// <summary>
    /// Идентификатор роли
    /// </summary>
    Guid RoleId { get; }
}