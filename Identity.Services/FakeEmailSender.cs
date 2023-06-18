using Identity.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace Identity.Services
{
    public sealed class FakeEmailSender : IEmailSender
    {
        private readonly ILogger _logger;

        public FakeEmailSender(ILogger<FakeEmailSender> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task SendAsync(string subject, string body, string to, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Fake Send: {To}, SUBJECT: {Subject} BODY: {Body}", to, subject, body);
            return Task.CompletedTask;
        }
    }
}