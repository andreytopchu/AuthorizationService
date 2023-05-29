using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Command.Policy;
using Identity.Application.Abstractions.Repositories.Policy;
using Identity.Application.Abstractions.UseCases;
using Identity.Application.Extensions;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications;
using Microsoft.Extensions.Internal;

namespace Identity.Application.UseCases.Policy;

public class DeletePolicyUseCase : IUseCase<IDeletePolicyCommand>
{
    private readonly ISystemClock _systemClock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPolicyWriteRepository _policyWriteRepository;

    public DeletePolicyUseCase(IUnitOfWork unitOfWork, IPolicyWriteRepository policyWriteRepository, ISystemClock systemClock)
    {
        _unitOfWork = unitOfWork;
        _policyWriteRepository = policyWriteRepository;
        _systemClock = systemClock;
    }

    public async Task Process(IDeletePolicyCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        var dbPolicy = await _policyWriteRepository.Read.SingleOrDefaultAsync(new EntityByKeySpecification<Domain.Entities.Policy, Guid>(arg.PolicyId),
            cancellationToken);

        if (dbPolicy is null)
            throw new PolicyNotFoundException(arg.PolicyId);

        dbPolicy.Name.ThrowIfPolicyIsFullAccess();

        if (dbPolicy.Roles.Any())
            throw new EntityInUseException<Domain.Entities.Policy>(arg.PolicyId);

        dbPolicy.Name = $"{dbPolicy.Id}.deleted#{dbPolicy.Name}";
        dbPolicy.DeletedUtc = _systemClock.UtcNow.UtcDateTime;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}