using Dex.Cap.Outbox.Models;

namespace Identity.Application.IntegrationEvents;

public class UserTokenInvalidationIntegrationEvent : BaseOutboxMessage
{
    public Guid[] UserIds { get; set; } = Array.Empty<Guid>();
}