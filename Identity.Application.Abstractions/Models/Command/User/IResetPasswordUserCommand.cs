using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.Abstractions.Models.Command.User;

/// <summary>
/// Интерфейс команды восстановления пароля пользователя
/// </summary>
public interface IResetPasswordUserCommand : IUseCaseArg
{
    /// <summary>
    /// Адрес эл.почты, на который высылается письмо с восстановлением пароля
    /// </summary>
    string Email { get; }

    IIdentityUriCommand? IdentityUri { get; set; }
}