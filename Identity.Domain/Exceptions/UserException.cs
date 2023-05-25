namespace Identity.Domain.Exceptions
{
    /// <remarks>Status: 400</remarks>
    public abstract class UserException : Exception
    {
        protected UserException()
        {
        }

        protected UserException(string? message) : base(message)
        {
        }

        protected UserException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}