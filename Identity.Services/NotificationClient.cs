using System.Collections.Immutable;
using Identity.Application.Abstractions.Models.Command.Email;
using Identity.Application.Abstractions.Services;
using MassTransit;

namespace Identity.Services
{
    public class NotificationClient : INotificationClient
    {
        private static readonly TimeSpan OperationTimeout = TimeSpan.FromMilliseconds(2000);
        private readonly ISendEndpointProvider _sendEndpoint;

        public NotificationClient(ISendEndpointProvider sendEndpoint)
        {
            _sendEndpoint = sendEndpoint ?? throw new ArgumentNullException(nameof(sendEndpoint));
        }

        public async Task SendEmail(IEnumerable<string> emails, string subject, string body, Guid? confirmationId = null,
            CancellationToken cancellationToken = default)
        {
            if (emails == null) throw new ArgumentNullException(nameof(emails));
            if (subject == null) throw new ArgumentNullException(nameof(subject));
            if (body == null) throw new ArgumentNullException(nameof(body));

            var arr = emails.ToImmutableArray();
            if (arr.Length > 0)
            {
                CheckLength(arr);

                var command = new SendEmailCommand(confirmationId ?? Guid.NewGuid(), subject, body, arr)
                {
                    ConfirmationRequired = confirmationId != null
                };

                await Send<ISendEmailCommand>(command, cancellationToken);
            }
        }

        private async Task Send<TMessage>(TMessage command, CancellationToken cancellationToken) where TMessage : class
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(OperationTimeout);
            try
            {
                await _sendEndpoint.Send(command, cts.Token);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException($"Timeout exceeded {OperationTimeout.TotalMilliseconds} msec");
            }
        }

        private static void CheckLength<T>(ImmutableArray<T> arr)
        {
            if (arr.Length > 100_00)
                throw new InvalidOperationException("count is limit at 10k");
        }
    }
}