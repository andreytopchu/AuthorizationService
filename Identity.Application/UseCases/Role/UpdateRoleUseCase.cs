using Identity.Application.Abstractions.Models.Command.Role;
using Identity.Application.Abstractions.Models.Query.Role;
using Identity.Application.Abstractions.Repositories.Role;
using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.UseCases.Role;

internal class UpdateRoleUseCase : IUseCase<IUpdateRoleCommand, RoleInfo>
{
    private readonly IRoleWriteRepository _roleWriteRepository;
    //private readonly IUserWriteRepository _userReadRepository;
    //private readonly IOutboxService<IUnitOfWork> _outboxService;

    public UpdateRoleUseCase(IRoleWriteRepository roleWriteRepository)//, IUserWriteRepository userWriteRepository,
        //IOutboxService<IUnitOfWork> outboxService)
    {
        _roleWriteRepository = roleWriteRepository ?? throw new ArgumentNullException(nameof(roleWriteRepository));
        //_userReadRepository = userWriteRepository ?? throw new ArgumentNullException(nameof(userWriteRepository));
        //_outboxService = outboxService ?? throw new ArgumentNullException(nameof(outboxService));
    }

    public async Task<RoleInfo> Process(IUpdateRoleCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null)
            throw new ArgumentNullException(nameof(arg));

        var roleId = arg.Id;
        //
        // roleId.ThrowIfRoleIdIsSuperAdmin();
        // roleId.ThrowIfRoleIdIsNoAccess();
        //
        // var correlationId = Guid.NewGuid();
        //
        // await _outboxService.ExecuteOperationAsync(correlationId, async (token, outboxContext) =>
        // {
        //     var dbRole = await _roleWriteRepository.Read.SingleOrDefaultAsync(new ActiveRoleSpecification(arg.Id), cancellationToken);
        //
        //     if (dbRole == null) throw new EntityNotFoundException<RoleInfo>(arg.Id.ToString());
        //
        //     dbRole.Policies ??= string.Empty;
        //     dbRole.Policies.Split(",").NotContainFullAccessPolicies();
        //
        //     var newPolicies = string.Join(',', arg.Policies.Distinct().OrderBy(x => x));
        //     var oldPolicies = string.Join(',', dbRole.Policies.Split(",").Distinct().OrderBy(x => x));
        //
        //     dbRole.Name = arg.Name;
        //     dbRole.Policies = newPolicies;
        //     dbRole.Description = arg.Description;
        //
        //     if (newPolicies != oldPolicies)
        //     {
        //         await outboxContext.EnqueueAsync(new UserTokenInvalidationIntegrationEvent
        //         {
        //             UserIds = await _userReadRepository.Read.GetUserIdsByRoleAsync(arg.Id, cancellationToken)
        //         }, token);
        //     }
        // }, cancellationToken);

        return await GetRoleById(roleId, cancellationToken);
    }

    private Task<RoleInfo> GetRoleById(Guid id, CancellationToken cancellationToken)
    {
        return _roleWriteRepository.Read.GetRoleByIdAsync<RoleInfo>(id, cancellationToken);
    }
}