namespace Identity.Domain.Exceptions
{
    public class EmailNotFoundException : NotFoundException
    {
        public EmailNotFoundException()
        {
        }

        public EmailNotFoundException(string message) : base(message)
        {
        }

        public EmailNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}