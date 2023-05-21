using Identity.Application.Abstractions.Models.Authorization;
using Identity.Application.Abstractions.Services;

namespace Identity.Services;

public class UserStoreService : IUserStoreService
{
    public async Task<AuthResult> ValidateCredentials(Credential credential)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> FindBySubjectId(Subject subject)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> FindByUsername(string username)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> FindByExternalProvider(string userId, string provider)
    {
        throw new NotImplementedException();
    }
}