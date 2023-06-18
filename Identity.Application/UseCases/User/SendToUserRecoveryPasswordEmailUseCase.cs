using Identity.Application.Abstractions.Models.Command.User;
using Identity.Application.Abstractions.Services;
using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.UseCases.User;

internal class SendToUserRecoveryPasswordEmailUseCase : IUseCase<ISendToUserRecoveryEmailCommand>
{
    private readonly INotificationClient _notificationClient;
    private readonly IEmailMessageBuilderByEmailType _emailMessageBuilderByEmailType;

    public SendToUserRecoveryPasswordEmailUseCase(INotificationClient notificationClient, IEmailMessageBuilderByEmailType emailMessageBuilderByEmailType)
    {
        _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
        _emailMessageBuilderByEmailType = emailMessageBuilderByEmailType ?? throw new ArgumentNullException(nameof(emailMessageBuilderByEmailType));
    }

    public async Task Process(ISendToUserRecoveryEmailCommand arg, CancellationToken cancellationToken)
    {
        if (arg.IdentityUri != null) await SendRegistryEmailByTypeAsync(arg.IdentityUri.Uri, arg.EmailType, arg.User, cancellationToken);

        await Task.CompletedTask;
    }

    private async Task SendRegistryEmailByTypeAsync(Uri baseUri, EmailType emailType, Domain.Entities.User user, CancellationToken cancellation)
    {
        if (baseUri == null) throw new ArgumentNullException(nameof(baseUri));
        if (user == null) throw new ArgumentNullException(nameof(user));

        var (subject, body) = await _emailMessageBuilderByEmailType.Build(baseUri, emailType, user, cancellation);

        await _notificationClient.SendEmail(new[] {user.Email}, subject, body, cancellationToken: cancellation);
    }
}