using MassTransit;

namespace Identity.Domain.Contracts.IntegrationEvents;

public interface IUserTokenInvalidationIntegrationEvent : IConsumer
{
    Guid[] UserIds { get; }
}