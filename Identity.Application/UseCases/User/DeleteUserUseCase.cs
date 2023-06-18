using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Command.User;
using Identity.Application.Abstractions.Models.Query.User;
using Identity.Application.Abstractions.Repositories.User;
using Identity.Application.Abstractions.UseCases;
using Identity.Application.Extensions;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications.User;
using Microsoft.Extensions.Internal;

namespace Identity.Application.UseCases.User;

internal class DeleteUserUseCase : IUseCase<IDeleteUserCommand>
{
    private readonly IUserWriteRepository _userWriteRepository;
    private readonly ISystemClock _systemClock;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserUseCase(IUnitOfWork unitOfWork, IUserWriteRepository userWriteRepository, ISystemClock systemClock)
    {
        _userWriteRepository = userWriteRepository ?? throw new ArgumentNullException(nameof(userWriteRepository));
        _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Process(IDeleteUserCommand arg, CancellationToken cancellationToken)
    {
        var dbUser = await _userWriteRepository.Read.SingleOrDefaultAsync(new UndeletedUserByIdSpecification(arg.UserId), cancellationToken);

        if (dbUser == null) throw new EntityNotFoundException<UserInfo>(arg.UserId);

        dbUser.RoleId.ThrowIfRoleIdIsSuperAdmin();

        dbUser.Email = $"{dbUser.Id}.deleted#{dbUser.Email}";
        dbUser.Password = "#deleted#";
        dbUser.DeletedUtc = _systemClock.UtcNow.UtcDateTime;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}