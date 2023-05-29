using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Command.Role;
using Identity.Application.Abstractions.Models.Query.Role;
using Identity.Application.Abstractions.Repositories.Role;
using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.UseCases.Role;

internal class AddRoleUseCase : IUseCase<IAddRoleCommand, RoleInfo>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleWriteRepository _roleWriteRepository;

    public AddRoleUseCase(IUnitOfWork unitOfWork, IRoleWriteRepository roleWriteRepository)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _roleWriteRepository = roleWriteRepository ?? throw new ArgumentNullException(nameof(roleWriteRepository));
    }

    public async Task<RoleInfo> Process(IAddRoleCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null)
            throw new ArgumentNullException(nameof(arg));

        // arg.PolicyIds.NotContainFullAccessPolicies();

        var newDbRecord = new Domain.Entities.Role
        {
            Name = arg.Name,
            CreatedUtc = DateTime.UtcNow,
            // Policies = string.Join(',', arg.Policies.Distinct()),
            Description = arg.Description
        };
        await _roleWriteRepository.AddAsync(newDbRecord, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetRoleById(newDbRecord.Id, cancellationToken);
    }

    private Task<RoleInfo> GetRoleById(Guid id, CancellationToken cancellationToken)
    {
        return _roleWriteRepository.Read.GetRoleByIdAsync<RoleInfo>(id, cancellationToken);
    }
}