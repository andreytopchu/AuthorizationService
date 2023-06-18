using AutoMapper;
using Dex.Cap.Outbox.Interfaces;
using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Command.User;
using Identity.Application.Abstractions.Models.Query.Role;
using Identity.Application.Abstractions.Models.Query.User;
using Identity.Application.Abstractions.Repositories.Role;
using Identity.Application.Abstractions.Repositories.User;
using Identity.Application.Abstractions.UseCases;
using Identity.Application.Extensions;
using Identity.Application.IntegrationEvents;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications.User;

namespace Identity.Application.UseCases.User;

internal class UpdateUserUseCase : IUseCase<IUpdateUserCommand, UserInfo>
{
    private readonly IUserWriteRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IOutboxService<IUnitOfWork> _outboxService;
    private readonly IRoleReadRepository _roleReadRepository;

    public UpdateUserUseCase(IRoleReadRepository roleReadRepository, IUserWriteRepository userRepository,
        IOutboxService<IUnitOfWork> outboxService, IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _outboxService = outboxService ?? throw new ArgumentNullException(nameof(outboxService));
        _roleReadRepository = roleReadRepository ?? throw new ArgumentNullException(nameof(roleReadRepository));
    }

    public async Task<UserInfo> Process(IUpdateUserCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        arg.RoleId.ThrowIfRoleIdIsSuperAdmin();

        await CheckUserRoleExists(arg.RoleId, cancellationToken);

        var correlationId = Guid.NewGuid();
        await _outboxService.ExecuteOperationAsync(correlationId, new {Mapper = _mapper}, async (token, cntx) =>
        {
            var userDb = await _userRepository.Read.SingleOrDefaultAsync(new ActiveUserByIdSpecification(arg.Id), cancellationToken);
            if (userDb == null) throw new EntityNotFoundException<UserInfo>(arg.Id);

            userDb.RoleId.ThrowIfRoleIdIsSuperAdmin();

            var oldRoleId = userDb.RoleId;
            cntx.State.Mapper.Map(arg, userDb);
            var newRoleId = userDb.RoleId;

            if (newRoleId != oldRoleId)
            {
                await cntx.EnqueueAsync(new UserTokenInvalidationIntegrationEvent {UserIds = new[] {arg.Id}}, token);
            }

            await cntx.DbContext.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        return await GetUserById(arg.Id, cancellationToken);
    }

    private async Task CheckUserRoleExists(Guid userRoleId, CancellationToken cancellationToken)
    {
        var isRoleExist = await _roleReadRepository.IsRoleExistAsync(userRoleId, cancellationToken);

        if (!isRoleExist)
            throw new EntityNotFoundException<RoleInfo>(userRoleId);
    }

    private Task<UserInfo> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        return _userRepository.Read.GetUserByIdAsync<UserInfo>(id, cancellationToken);
    }
}