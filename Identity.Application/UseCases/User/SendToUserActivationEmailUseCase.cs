using Identity.Application.Abstractions.Models.Command.User;
using Identity.Application.Abstractions.Services;
using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.UseCases.User;

// ReSharper disable once UnusedType.Global
internal class SendToUserActivationEmailUseCase : IUseCase<ISendToUserActivationEmailCommand>
{
    private readonly INotificationClient _notificationClient;
    private readonly IEmailMessageBuilderByEmailType _emailMessageBuilderByEmailType;

    public SendToUserActivationEmailUseCase(INotificationClient notificationClient,
        IEmailMessageBuilderByEmailType emailMessageBuilderByEmailType)
    {
        _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
        _emailMessageBuilderByEmailType = emailMessageBuilderByEmailType ?? throw new ArgumentNullException(nameof(emailMessageBuilderByEmailType));
    }

    public async Task Process(ISendToUserActivationEmailCommand arg, CancellationToken cancellationToken)
    {
        await SendRegistryEmailByTypeAsync(arg.EmailType, arg.User, cancellationToken);

        await Task.CompletedTask;
    }

    private async Task SendRegistryEmailByTypeAsync(EmailType emailType, Domain.Entities.User user, CancellationToken cancellation)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        var (subject, body) = await _emailMessageBuilderByEmailType.Build(emailType, user, cancellation);

        await _notificationClient.SendEmail(new[] {user.Email}, subject, body, cancellationToken: cancellation);
    }
}