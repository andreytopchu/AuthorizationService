namespace Identity.Domain.Exceptions
{
    /// <summary>
    /// Status:412
    /// </summary>
    public abstract class PreconditionFailedException : UserException
    {
        protected PreconditionFailedException()
        {
        }

        protected PreconditionFailedException(string? message) : base(message)
        {
        }

        protected PreconditionFailedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}