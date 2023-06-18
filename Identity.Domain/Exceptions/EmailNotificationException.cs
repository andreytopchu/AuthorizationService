namespace Identity.Domain.Exceptions
{
    public sealed class EmailNotificationException : Exception
    {
        public EmailNotificationException()
        {
        }

        public EmailNotificationException(string message) : base(message)
        {
        }

        public EmailNotificationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}