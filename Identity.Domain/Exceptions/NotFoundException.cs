namespace Identity.Domain.Exceptions
{
    /// <remarks>Status: 404</remarks>
    public abstract class NotFoundException : UserException
    {
        protected NotFoundException()
        {
        }

        protected NotFoundException(string? message) : base(message)
        {
        }

        protected NotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}