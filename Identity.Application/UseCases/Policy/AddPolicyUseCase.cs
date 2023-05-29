using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Command.Policy;
using Identity.Application.Abstractions.Models.Query.Policy;
using Identity.Application.Abstractions.Repositories.Policy;
using Identity.Application.Abstractions.UseCases;
using Identity.Domain.Entities;

namespace Identity.Application.UseCases.Policy;

public class AddPolicyUseCase : IUseCase<IAddPolicyCommand, PolicyInfo>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPolicyWriteRepository _policyWriteRepository;

    public AddPolicyUseCase(IUnitOfWork unitOfWork, IPolicyWriteRepository policyWriteRepository)
    {
        _unitOfWork = unitOfWork;
        _policyWriteRepository = policyWriteRepository;
    }

    public async Task<PolicyInfo> Process(IAddPolicyCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        var newDbRecord = new Domain.Entities.Policy
        {
            Name = arg.Name,
            CreatedUtc = DateTime.UtcNow,
            Clients = arg.ClientIds.Select(x => new ClientPolicy {ClientId = x, PolicyName = string.Concat(new[] {x, "_", arg.Name})}).ToArray()
        };

        await _policyWriteRepository.AddAsync(newDbRecord, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetPolicyById(newDbRecord.Id, cancellationToken);
    }

    private Task<PolicyInfo> GetPolicyById(Guid id, CancellationToken cancellationToken)
    {
        return _policyWriteRepository.Read.GetPolicyByIdAsync<PolicyInfo>(id, cancellationToken);
    }
}