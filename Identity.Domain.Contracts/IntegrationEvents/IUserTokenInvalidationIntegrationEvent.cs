namespace Identity.Domain.Contracts.IntegrationEvents;

public interface IUserTokenInvalidationIntegrationEvent
{
    Guid[] UserIds { get; }
}