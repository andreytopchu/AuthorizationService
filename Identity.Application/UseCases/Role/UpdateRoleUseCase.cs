using AutoMapper;
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
using Identity.Domain.Specifications.Policy;
using Identity.Domain.Specifications.Role;

namespace Identity.Application.UseCases.Role;

internal class UpdateRoleUseCase : IUseCase<IUpdateRoleCommand, RoleInfo>
{
    private readonly IRoleWriteRepository _roleWriteRepository;
    private readonly IUserReadRepository _userReadRepository;
    private readonly IPolicyWriteRepository _policyWriteRepository;
    private readonly IOutboxService<IUnitOfWork> _outboxService;
    private readonly IMapper _mapper;

    public UpdateRoleUseCase(IRoleWriteRepository roleWriteRepository, IUserReadRepository userReadRepository, IPolicyWriteRepository policyWriteRepository,
        IOutboxService<IUnitOfWork> outboxService, IMapper mapper)
    {
        _roleWriteRepository = roleWriteRepository;
        _userReadRepository = userReadRepository;
        _policyWriteRepository = policyWriteRepository;
        _outboxService = outboxService;
        _mapper = mapper;
    }

    public async Task<RoleInfo> Process(IUpdateRoleCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null)
            throw new ArgumentNullException(nameof(arg));

        var roleId = arg.Id;

        roleId.ThrowIfRoleIdIsSuperAdmin();

        var correlationId = Guid.NewGuid();

        await _outboxService.ExecuteOperationAsync(correlationId, async (token, outboxContext) =>
        {
            var dbRole = await _roleWriteRepository.Read.SingleOrDefaultAsync(new ActiveRoleSpecification(arg.Id), cancellationToken);

            if (dbRole == null) throw new EntityNotFoundException<RoleInfo>(arg.Id.ToString());

            dbRole.Policies.ForEach(x => x.Name.ThrowIfPolicyIsFullAccess());

            var policies = await _policyWriteRepository.Read.FilterAsync(new PolicyByIdsSpecification(arg.PolicyIds), cancellationToken);
            if (policies.Length != arg.PolicyIds.Length)
            {
                throw new ThereAreUnacceptablePolicies(arg.PolicyIds);
            }

            policies.ForEach(x => x.Name.ThrowIfPolicyIsFullAccess());

            _mapper.Map(arg, dbRole);

            dbRole.Policies = policies.ToList();

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