namespace Identity.Domain.Exceptions;

/// <summary>
/// Пользователь не найден
/// Status:404
/// </summary>
public sealed class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(Guid userId)
    {
        Data[nameof(userId)] = userId;
    }
}