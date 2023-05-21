using Identity.Application.Abstractions.Models.Authorization;

namespace Identity.Application.Abstractions.Services;

public interface IUserStoreService
{
    public Task<AuthResult> ValidateCredentials(Credential credential);

    public Task<User?> FindBySubjectId(Subject subject);

    public Task<User?> FindByUsername(string username);

    public Task<User?> FindByExternalProvider(string userId, string provider);
}