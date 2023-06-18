namespace Identity.Application.Abstractions.Services
{
    public interface INotificationClient
    {
        /// <exception cref="TimeoutException"/>
        /// <exception cref="OperationCanceledException"/>
        Task SendEmail(IEnumerable<string> emails, string subject, string body, Guid? confirmationId = null,
            CancellationToken cancellationToken = default);
    }
}