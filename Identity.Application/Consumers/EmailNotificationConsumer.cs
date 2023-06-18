using System.Diagnostics;
using Identity.Application.Abstractions.Models.Command.Email;
using Identity.Application.Abstractions.Services;
using Identity.Domain.Exceptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Consumers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal sealed class EmailNotificationConsumer : BaseConsumer<ISendEmailCommand>
    {
        private readonly IEmailSender _emailSender;

        private const int SendEmailDelay = 1000;

        public EmailNotificationConsumer(IEmailSender emailSender, ILogger<EmailNotificationConsumer> logger)
            : base(logger)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        }

        protected override async Task Process(ConsumeContext<ISendEmailCommand> context)
        {
            Debug.Assert(context != null);

            var message = context.Message;

            Logger.LogInformation("Consumed 'E-Mail' message {RequestId} to send for {Count} recipients", message.RequestId, message.Emails.Count);

            foreach (var email in message.Emails)
            {
                try
                {
                    await _emailSender.SendAsync(message.Heading, message.MessageText, email.Trim(), context.CancellationToken);
                    Logger.LogInformation("'E-Mail' {RequestId} successfully sended for '{Email}'", message.RequestId, email);
                }
                catch (EmailNotificationException ex)
                {
                    Logger.LogError(ex, "Failed to send 'E-Mail' {RequestId} for '{Email}'", message.RequestId, email);
                    continue;
                }

                await Task.Delay(SendEmailDelay, context.CancellationToken);
            }
        }
    }
}