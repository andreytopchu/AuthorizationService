// ReSharper properties

using Identity.Application.Abstractions.Models.Command.User;

namespace Identity.Application.Abstractions.Services;

/// <summary>
/// Построитель сообщения письма электронной почты
/// </summary>
public interface IEmailMessageBuilderByEmailType
{
    /// <summary>
    /// Построить сообщение письма электронной почты
    /// </summary>
    /// <param name="user"></param>
    /// <param name="emailType"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public Task<(string subject, string body)> Build(EmailType emailType, Domain.Entities.User user, CancellationToken cancellation);
}