using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.Abstractions.Models.Command.User;

/// <summary>
/// Интерфейс команды подтверждение пароля пользователя при регистрации
/// </summary>
public interface IAcceptUserCommand : IUseCaseArg
{
    public string Password { get; init; }

    /// <summary>
    /// Токен, получаемый из URL при открытии письма, либо в БД
    /// </summary>
    public string Token { get; init; }

    public EmailType EmailType { get; set; }
}