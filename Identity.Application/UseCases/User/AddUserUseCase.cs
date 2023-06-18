using AutoMapper;
using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Command.User;
using Identity.Application.Abstractions.Models.Query.Role;
using Identity.Application.Abstractions.Models.Query.User;
using Identity.Application.Abstractions.Repositories.Role;
using Identity.Application.Abstractions.Repositories.User;
using Identity.Application.Abstractions.UseCases;
using Identity.Application.Extensions;
using Identity.Domain.Exceptions;

namespace Identity.Application.UseCases.User;

public class AddUserUseCase : IUseCase<IAddUserCommand, UserInfo>
{
    private readonly IUseCase<ISendToUserActivationEmailCommand> _emailNotificationService;
    private readonly IUserWriteRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleReadRepository _roleReadRepository;

    public AddUserUseCase(IUnitOfWork unitOfWork, IRoleReadRepository roleReadRepository, IUserWriteRepository userRepository, IMapper mapper,
        IUseCase<ISendToUserActivationEmailCommand> emailNotificationService)

    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _emailNotificationService = emailNotificationService ?? throw new ArgumentNullException(nameof(emailNotificationService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _roleReadRepository = roleReadRepository ?? throw new ArgumentNullException(nameof(roleReadRepository));
    }

    public async Task<UserInfo> Process(IAddUserCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        arg.RoleId.ThrowIfRoleIdIsSuperAdmin();

        await CheckUserRoleExists(arg.RoleId, cancellationToken);

        var userDb = _mapper.Map(arg, new Domain.Entities.User {Password = "UserNotActivated"});
        await _userRepository.AddAsync(userDb, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (arg.IdentityUri != null)
        {
            await _emailNotificationService.Process(new SendToUserActivationEmailRequest
            {
                User = userDb,
                IdentityUri = arg.IdentityUri,
                EmailType = EmailType.RegisterUser
            }, cancellationToken);
        }

        return await GetUserById(userDb.Id, cancellationToken);
    }

    private async Task CheckUserRoleExists(Guid userRoleId, CancellationToken cancellationToken)
    {
        var isRoleExist = await _roleReadRepository.IsRoleExistAsync(userRoleId, cancellationToken);

        if (!isRoleExist) throw new EntityNotFoundException<RoleInfo>(userRoleId);
    }

    private Task<UserInfo> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        return _userRepository.Read.GetUserByIdAsync<UserInfo>(id, cancellationToken);
    }

    private class SendToUserActivationEmailRequest : ISendToUserActivationEmailCommand
    {
        public Domain.Entities.User User { get; init; } = null!;
        public IIdentityUriCommand? IdentityUri { get; set; }

        public EmailType EmailType { get; init; }
    }
}