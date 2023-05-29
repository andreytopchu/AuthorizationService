using System.Security.Authentication;

namespace Identity.Application.Abstractions.Services;

public interface IUserIdService
{
    /// <exception cref="AuthenticationException"/>
    Guid UserId { get; }

    bool TryGetUserId(out Guid? userId);
}