using AutoMapper;
using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Command.Policy;
using Identity.Application.Abstractions.Models.Query.Policy;
using Identity.Application.Abstractions.Repositories.Policy;
using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.UseCases.Policy;

public class AddPolicyUseCase : IUseCase<IAddPolicyCommand, PolicyInfo>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPolicyWriteRepository _policyWriteRepository;
    private readonly IMapper _mapper;

    public AddPolicyUseCase(IUnitOfWork unitOfWork, IPolicyWriteRepository policyWriteRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _policyWriteRepository = policyWriteRepository;
        _mapper = mapper;
    }

    public async Task<PolicyInfo> Process(IAddPolicyCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        var policy = _mapper.Map<Domain.Entities.Policy>(arg);

        await _policyWriteRepository.AddAsync(policy, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetPolicyById(policy.Id, cancellationToken);
    }

    private Task<PolicyInfo> GetPolicyById(Guid id, CancellationToken cancellationToken)
    {
        return _policyWriteRepository.Read.GetByIdAsync<PolicyInfo>(id, cancellationToken);
    }
}