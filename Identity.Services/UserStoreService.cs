using AutoMapper;
using Dex.Extensions;
using Identity.Application.Abstractions.Extensions;
using Identity.Application.Abstractions.Models.Authorization;
using Identity.Application.Abstractions.Repositories.User;
using Identity.Application.Abstractions.Services;
using Identity.Domain.Constants;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications.User;
using IdentityServer4.Extensions;

namespace Identity.Services;

internal class UserStoreService : IUserStoreService
{
    private readonly IUserReadRepository _userRepository;
    private readonly IPasswordHashGenerator _passwordHashGenerator;
    private readonly IMapper _mapper;

    public UserStoreService(IUserReadRepository userRepository, IPasswordHashGenerator passwordHashGenerator, IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHashGenerator = passwordHashGenerator;
        _mapper = mapper;
    }

    public async Task<AuthResult> ValidateCredentials(Credential credential)
    {
        if (credential == null) throw new ArgumentNullException(nameof(credential));

        try
        {
            var user = await _userRepository.GetBySpecAsync(new ActiveUserByEmailSpecification(credential.Username), CancellationToken.None);
            return user.Password == _passwordHashGenerator.MakeHashWithSalt(user.Id, credential.Password)
                ? new AuthResult {Success = true, Sub = user.Id.ToString()}
                : new AuthResult {Message = ValidationErrors.InvalidCredentials};
        }
        catch (EntityNotFoundException)
        {
            return new AuthResult {Message = ValidationErrors.InvalidCredentials};
        }
    }

    public async Task<User?> FindBySubjectId(Subject subject)
    {
        if (subject == null) throw new ArgumentNullException(nameof(subject));

        if (!Guid.TryParse((ReadOnlySpan<char>) subject.Sub, out var sub))
        {
            throw new InvalidOperationException($"sub is invalid: {subject.Sub}");
        }

        var user = await _userRepository.GetActiveUserByIdAsync(sub, CancellationToken.None);

        var foundUser = _mapper.Map<User>(user);
        foundUser.Claims.AddRange(FillClaims(user, subject.Claims));
        return foundUser;
    }

    public async Task<User?> FindByUsername(string username)
    {
        if (username.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(username));

        var user = await _userRepository.GetBySpecAsync(new ActiveUserByEmailSpecification(username), CancellationToken.None);

        return _mapper.Map<User>(user);
    }

    private static IEnumerable<SimpleClaim> FillClaims(Domain.Entities.User user, IEnumerable<SimpleClaim> requestClaims)
    {
        foreach (var claim in requestClaims)
        {
            switch (claim.Type)
            {
                case "name":
                {
                    var value = user.GetFullName();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        yield return new SimpleClaim {Type = claim.Type, Value = value};
                    }

                    break;
                }
                case "policy":
                {
                    if (user.Role is not null && !IEnumerableExtensions.IsNullOrEmpty(user.Role.Policies))
                    {
                        var policies = new List<string>();

                        foreach (var policy in user.Role.Policies)
                        {
                            policies.AddRange(policy.ApiResources.Select(x=>x.PolicyName));
                        }

                        yield return new SimpleClaim {Type = claim.Type, Value = string.Join(',', policies)};
                    }

                    break;
                }
            }
        }
    }
}