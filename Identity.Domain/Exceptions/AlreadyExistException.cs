namespace Identity.Domain.Exceptions;

/// <remarks>Status: 409</remarks>
public abstract class AlreadyExistException : UserException
{
    protected AlreadyExistException()
    {
    }

    protected AlreadyExistException(string? message) : base(message)
    {
    }

    protected AlreadyExistException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}