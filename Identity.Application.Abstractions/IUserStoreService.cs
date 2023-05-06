using Identity.Application.Abstractions.Models;

namespace Identity.Application.Abstractions;

public interface IUserStoreService
{
    public Task<AuthResult> ValidateCredentials(Credential credential);

    public Task<User?> FindBySubjectId(Subject subject);

    public Task<User?> FindByUsername(string username);

    public Task<User?> FindByExternalProvider(string userId, string provider);
}