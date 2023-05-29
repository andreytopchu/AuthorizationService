using Dex.Cap.Outbox.Interfaces;
using Dex.Extensions;
using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Command.Role;
using Identity.Application.Abstractions.Models.Query.Role;
using Identity.Application.Abstractions.Repositories.Policy;
using Identity.Application.Abstractions.Repositories.Role;
using Identity.Application.Abstractions.Repositories.User;
using Identity.Application.Abstractions.UseCases;
using Identity.Application.Extensions;
using Identity.Application.IntegrationEvents;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications.Role;

namespace Identity.Application.UseCases.Role;

internal class UpdateRoleUseCase : IUseCase<IUpdateRoleCommand, RoleInfo>
{
    private readonly IRoleWriteRepository _roleWriteRepository;
    private readonly IUserReadRepository _userReadRepository;
    private readonly IPolicyReadRepository _policyReadRepository;
    private readonly IOutboxService<IUnitOfWork> _outboxService;

    public UpdateRoleUseCase(IRoleWriteRepository roleWriteRepository, IUserReadRepository userReadRepository, IPolicyReadRepository policyReadRepository,
        IOutboxService<IUnitOfWork> outboxService)
    {
        _roleWriteRepository = roleWriteRepository;
        _userReadRepository = userReadRepository;
        _policyReadRepository = policyReadRepository;
        _outboxService = outboxService;
    }

    public async Task<RoleInfo> Process(IUpdateRoleCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null)
            throw new ArgumentNullException(nameof(arg));

        var roleId = arg.Id;

        roleId.ThrowIfRoleIdIsSuperAdmin();
        roleId.ThrowIfRoleIdIsNoAccess();

        var correlationId = Guid.NewGuid();

        await _outboxService.ExecuteOperationAsync(correlationId, async (token, outboxContext) =>
        {
            var dbRole = await _roleWriteRepository.Read.SingleOrDefaultAsync(new ActiveRoleSpecification(arg.Id), cancellationToken);

            if (dbRole == null) throw new EntityNotFoundException<RoleInfo>(arg.Id.ToString());

            dbRole.Policies.ForEach(x => x.Name.ThrowIfPolicyIsFullAccess());

            var policies = await _policyReadRepository.GetPolicies<Domain.Entities.Policy>(arg.PolicyIds, cancellationToken);
            if (policies.Length != arg.PolicyIds.Length)
            {
                throw new ThereAreUnacceptablePolicies(arg.PolicyIds);
            }

            policies.ForEach(x => x.Name.ThrowIfPolicyIsFullAccess());

            dbRole.Name = arg.Name;
            dbRole.Policies = policies;
            dbRole.Description = arg.Description;


            await outboxContext.EnqueueAsync(new UserTokenInvalidationIntegrationEvent
            {
                UserIds = await _userReadRepository.GetUserIdsByRoleAsync(arg.Id, cancellationToken)
            }, token);
        }, cancellationToken);

        return await GetRoleById(roleId, cancellationToken);
    }

    private Task<RoleInfo> GetRoleById(Guid id, CancellationToken cancellationToken)
    {
        return _roleWriteRepository.Read.GetRoleByIdAsync<RoleInfo>(id, cancellationToken);
    }
}