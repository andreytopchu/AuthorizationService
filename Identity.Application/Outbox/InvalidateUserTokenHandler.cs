using Dex.Cap.Outbox.Interfaces;
using Identity.Application.Abstractions.Services;
using Identity.Application.IntegrationEvents;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Outbox;

internal class InvalidateUserTokenHandler : IOutboxMessageHandler<UserTokenInvalidationIntegrationEvent>
{
    private readonly IInvalidateUserTokenService _invalidateUserTokenService;
    private readonly ILogger<InvalidateUserTokenHandler> _logger;

    public InvalidateUserTokenHandler(IInvalidateUserTokenService invalidateUserTokenService, ILogger<InvalidateUserTokenHandler> logger)
    {
        _invalidateUserTokenService = invalidateUserTokenService;
        _logger = logger;
    }

    public async Task ProcessMessage(UserTokenInvalidationIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (integrationEvent == null)
        {
            throw new ArgumentNullException(nameof(integrationEvent));
        }

        if (integrationEvent.UserIds.Any())
        {
            await _invalidateUserTokenService.InvalidateToken(integrationEvent.UserIds, cancellationToken);
            _logger.LogDebug("Invalidated tokens for users {UserIds}", integrationEvent.UserIds);
        }
        else
        {
            _logger.LogDebug("No users for invalidate token");
        }
    }

    public Task ProcessMessage(IOutboxMessage outbox, CancellationToken cancellationToken)
    {
        return ProcessMessage((UserTokenInvalidationIntegrationEvent) outbox, cancellationToken);
    }
}