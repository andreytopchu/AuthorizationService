using Dex.SecurityTokenProvider.Interfaces;
using Identity.Abstractions;
using Identity.Application.Abstractions.Extensions;
using Identity.Application.Abstractions.Models.Command.User;
using Identity.Application.Abstractions.Models.Tokens;
using Identity.Application.Abstractions.Options;
using Identity.Application.Abstractions.Repositories.User;
using Identity.Application.Abstractions.Services;
using Identity.Application.Abstractions.UseCases;
using Identity.Domain.Exceptions;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;

namespace Identity.Application.UseCases.User;

internal class UserRestorePasswordUseCase : IUseCase<IUserRestorePasswordCommand>
{
    private readonly IUserReadRepository _userReadRepository;
    private readonly IPasswordHashGenerator _passwordHashGenerator;
    private readonly ISystemClock _systemClock;
    private readonly IOptions<TokenOptions> _tokenOptions;
    private readonly ITokenProvider _tokenProvider;
    private readonly IUnitOfWork _unitOfWork;

    public UserRestorePasswordUseCase(IUserReadRepository userReadRepository, IPasswordHashGenerator passwordHashGenerator, ISystemClock systemClock,
        IOptions<TokenOptions> tokenOptions, ITokenProvider tokenProvider, IUnitOfWork unitOfWork)
    {
        _userReadRepository = userReadRepository ?? throw new ArgumentNullException(nameof(userReadRepository));
        _passwordHashGenerator = passwordHashGenerator ?? throw new ArgumentNullException(nameof(passwordHashGenerator));
        _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        _tokenOptions = tokenOptions ?? throw new ArgumentNullException(nameof(tokenOptions));
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Process(IUserRestorePasswordCommand arg, CancellationToken cancellationToken)
    {
        if (arg.Token == null) throw new ArgumentNullException(nameof(arg));

        var userDb = await GetUserByToken(arg.Token, cancellationToken);

        userDb.Password = _passwordHashGenerator.MakeHashWithSalt(userDb.Id, arg.Password);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Domain.Entities.User> GetUserByToken(string token, CancellationToken cancellation)
    {
        var tokenData = await _tokenProvider.GetTokenDataFromUrlAsync<RecoveryUserPasswordToken>(token, cancellationToken: cancellation);

        var userDb = await _userReadRepository.GetUserByIdAsync<Domain.Entities.User>(tokenData.UserId, cancellation);
        if (userDb is not {DeletedUtc: null})
            throw new EntityNotFoundException<Domain.Entities.User>(tokenData.UserId);

        if (_systemClock.UtcNow.UtcDateTime - tokenData.Created > _tokenOptions.Value.ForgotTokenLifeDays)
            throw new InvalidPreconditionException<Domain.Entities.User>(userDb.Id, "Token expired");

        await _tokenProvider.MarkTokenAsUsed(tokenData.Id);

        return userDb;
    }
}