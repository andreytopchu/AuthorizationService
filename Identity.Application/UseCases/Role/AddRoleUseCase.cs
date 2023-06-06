using AutoMapper;
using Dex.Extensions;
using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Command.Role;
using Identity.Application.Abstractions.Models.Query.Role;
using Identity.Application.Abstractions.Repositories.Policy;
using Identity.Application.Abstractions.Repositories.Role;
using Identity.Application.Abstractions.UseCases;
using Identity.Application.Extensions;
using Identity.Domain.Exceptions;

namespace Identity.Application.UseCases.Role;

internal class AddRoleUseCase : IUseCase<IAddRoleCommand, RoleInfo>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleWriteRepository _roleWriteRepository;
    private readonly IPolicyReadRepository _policyReadRepository;
    private readonly IMapper _mapper;

    public AddRoleUseCase(IUnitOfWork unitOfWork, IRoleWriteRepository roleWriteRepository, IPolicyReadRepository policyReadRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _roleWriteRepository = roleWriteRepository;
        _policyReadRepository = policyReadRepository;
        _mapper = mapper;
    }

    public async Task<RoleInfo> Process(IAddRoleCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        var policies = await _policyReadRepository.GetPolicies<Domain.Entities.Policy>(arg.PolicyIds, cancellationToken);
        if (policies.Length != arg.PolicyIds.Length)
        {
            throw new ThereAreUnacceptablePolicies(arg.PolicyIds);
        }

        policies.ForEach(x => x.Name.ThrowIfPolicyIsFullAccess());

        var role = _mapper.Map<Domain.Entities.Role>(arg);

        role.Policies = policies;

        await _roleWriteRepository.AddAsync(role, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetRoleById(role.Id, cancellationToken);
    }

    private Task<RoleInfo> GetRoleById(Guid id, CancellationToken cancellationToken)
    {
        return _roleWriteRepository.Read.GetRoleByIdAsync<RoleInfo>(id, cancellationToken);
    }
}