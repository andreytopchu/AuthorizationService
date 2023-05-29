using Dex.Cap.Outbox.Models;
using Identity.Domain.Contracts.IntegrationEvents;

namespace Identity.Application.IntegrationEvents;

public class UserTokenInvalidationIntegrationEvent : BaseOutboxMessage, IUserTokenInvalidationIntegrationEvent
{
    public Guid[] UserIds { get; set; } = Array.Empty<Guid>();
}