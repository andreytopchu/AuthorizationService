using System;
using System.Threading.Tasks;
using Identity.Domain.Contracts.IntegrationEvents;
using Identity.Services;
using MassTransit;

namespace Identity.Consumers
{
    internal sealed class InvalidateUserTokenConsumer : IConsumer<IUserTokenInvalidationIntegrationEvent>
    {
        private readonly IInvalidateUserTokenService _invalidateUserToken;

        public InvalidateUserTokenConsumer(IInvalidateUserTokenService invalidateUserToken)
        {
            _invalidateUserToken = invalidateUserToken ?? throw new ArgumentNullException(nameof(invalidateUserToken));
        }

        public Task Consume(ConsumeContext<IUserTokenInvalidationIntegrationEvent> context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var userIds = context.Message.UserIds;

            return _invalidateUserToken.InvalidateToken(userIds, context.CancellationToken);
        }
    }
}