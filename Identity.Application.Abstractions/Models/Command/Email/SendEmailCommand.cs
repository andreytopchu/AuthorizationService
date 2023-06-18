using System.Collections.Immutable;
using Identity.Application.Abstractions.Extensions;

namespace Identity.Application.Abstractions.Models.Command.Email
{
    public sealed class SendEmailCommand : ISendEmailCommand
    {
        public SendEmailCommand(Guid requestId, string heading, string message, IEnumerable<string> emails)
        {
            if (emails is null)
            {
                throw new ArgumentNullException(nameof(emails));
            }

            RequestId = requestId;
            Heading = heading.NotNullParam(nameof(heading));
            MessageText = message ?? throw new ArgumentNullException(nameof(message));
            Emails = emails.ToImmutableList();

            if (Emails.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Emails), "The email list was empty");
            }
        }

        public Guid RequestId { get; }

        public string Heading { get; }

        public string MessageText { get; }

        public IReadOnlyCollection<string> Emails { get; }

        public bool ConfirmationRequired { get; init; }
    }
}