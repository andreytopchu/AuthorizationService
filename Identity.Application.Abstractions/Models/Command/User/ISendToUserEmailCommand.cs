using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.Abstractions.Models.Command.User;

/// <summary>
/// Интерфейс команды отправки письма пользователю
/// </summary>
public interface ISendToUserEmailCommand : IUseCaseArg
{
    Domain.Entities.User User { get; }

    IIdentityUriCommand? IdentityUri { get; set; }

    public EmailType EmailType { get; init; }
}

/// <summary>
/// Типы писем для отправки пользователю
/// </summary>
public enum EmailType
{
    RegisterUser,
    RecoveryPassword
}