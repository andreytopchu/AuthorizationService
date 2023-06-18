namespace Identity.Application.Abstractions.Services
{
    public interface IEmailSender
    {
        /// <exception cref="OperationCanceledException"/>
        /// <exception cref="EmailNotificationException"/>
        Task SendAsync(string subject, string body, string to, CancellationToken cancellationToken = default);
    }
}