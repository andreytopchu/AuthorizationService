using AutoMapper;
using Dex.Cap.Outbox.Interfaces;
using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Command.Policy;
using Identity.Application.Abstractions.Models.Query.Policy;
using Identity.Application.Abstractions.Repositories.Policy;
using Identity.Application.Abstractions.Repositories.User;
using Identity.Application.Abstractions.UseCases;
using Identity.Application.Extensions;
using Identity.Application.IntegrationEvents;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications;

namespace Identity.Application.UseCases.Policy;

public class UpdatePolicyUseCase : IUseCase<IUpdatePolicyCommand, PolicyInfo>
{
    private readonly IPolicyWriteRepository _policyWriteRepository;
    private readonly IUserReadRepository _userReadRepository;
    private readonly IOutboxService<IUnitOfWork> _outboxService;
    private readonly IMapper _mapper;

    public UpdatePolicyUseCase(IPolicyWriteRepository policyWriteRepository, IOutboxService<IUnitOfWork> outboxService, IUserReadRepository userReadRepository,
        IMapper mapper)
    {
        _policyWriteRepository = policyWriteRepository;
        _outboxService = outboxService;
        _userReadRepository = userReadRepository;
        _mapper = mapper;
    }

    public async Task<PolicyInfo> Process(IUpdatePolicyCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        arg.Name.ThrowIfPolicyIsFullAccess();

        var correlationId = Guid.NewGuid();

        await _outboxService.ExecuteOperationAsync(correlationId, async (token, outboxContext) =>
        {
            var dbPolicy = await _policyWriteRepository.Read.SingleOrDefaultAsync(new EntityByKeySpecification<Domain.Entities.Policy, Guid>(arg.Id),
                cancellationToken);

            if (dbPolicy is null)
                throw new PolicyNotFoundException(arg.Id);

            dbPolicy.Name.ThrowIfPolicyIsFullAccess();

            var oldName = dbPolicy.Name;

            _mapper.Map(arg, dbPolicy);

            if (oldName != arg.Name)
            {
                await outboxContext.EnqueueAsync(new UserTokenInvalidationIntegrationEvent()
                {
                    UserIds = await _userReadRepository.GetUserIdsByPolicyAsync(arg.Id, cancellationToken)
                }, token);
            }
        }, cancellationToken);

        return await GetPolicyById(arg.Id, cancellationToken);
    }

    private Task<PolicyInfo> GetPolicyById(Guid id, CancellationToken cancellationToken)
    {
        return _policyWriteRepository.Read.GetByIdAsync<PolicyInfo>(id, cancellationToken);
    }
}