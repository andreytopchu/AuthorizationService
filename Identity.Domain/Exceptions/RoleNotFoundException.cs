namespace Identity.Domain.Exceptions;

/// <summary>
/// Роль не найдена
/// Status:404
/// </summary>
public sealed class RoleNotFoundException : NotFoundException
{
    public RoleNotFoundException(Guid roleId)
    {
        Data[nameof(roleId)] = roleId;
    }
}