namespace Identity.Domain.Exceptions
{
    /// <summary>
    /// Не выполнены условия для обработки запроса.
    /// </summary>
    /// <remarks>Status: 412</remarks>
    public sealed class InvalidPreconditionException<T> : PreconditionFailedException
    {
        public InvalidPreconditionException(object key, string? message) : base(message)
        {
            Data["Key"] = key;
            Data["Type"] = typeof(T).Name;
        }
    }
}