using Dex.Extensions;
using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Command.Role;
using Identity.Application.Abstractions.Models.Query.Role;
using Identity.Application.Abstractions.Repositories.Role;
using Identity.Application.Abstractions.Repositories.User;
using Identity.Application.Abstractions.UseCases;
using Identity.Application.Extensions;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications.Role;
using Microsoft.Extensions.Internal;

namespace Identity.Application.UseCases.Role;

internal class DeleteRoleUseCase : IUseCase<IDeleteRoleCommand>
{
    private readonly IUserReadRepository _userReadRepository;
    private readonly ISystemClock _systemClock;
    private readonly IRoleWriteRepository _roleWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoleUseCase(IUnitOfWork unitOfWork, IUserReadRepository userReadRepository, ISystemClock systemClock,
        IRoleWriteRepository roleWriteRepository)
    {
        _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        _roleWriteRepository = roleWriteRepository ?? throw new ArgumentNullException(nameof(roleWriteRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userReadRepository = userReadRepository ?? throw new ArgumentNullException(nameof(userReadRepository));
    }

    public async Task Process(IDeleteRoleCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        arg.RoleId.ThrowIfRoleIdIsSuperAdmin();

        var userIds = await _userReadRepository.GetUserIdsByRoleAsync(arg.RoleId, cancellationToken);

        if (userIds.Any())
        {
            throw new EntityInUseException<RoleInfo>(arg.RoleId);
        }

        var dbRole = await _roleWriteRepository.Read.SingleOrDefaultAsync(new ActiveRoleSpecification(arg.RoleId), cancellationToken);
        if (dbRole == null)
            throw new EntityNotFoundException<RoleInfo>(arg.RoleId);

        dbRole.Policies.ForEach(x => x.Name.ThrowIfPolicyIsFullAccess());

        dbRole.Name = $"{dbRole.Id}.deleted#{dbRole.Name}";
        dbRole.DeletedUtc = _systemClock.UtcNow.UtcDateTime;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}