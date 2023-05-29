namespace Identity.Domain.Exceptions
{
    /// <summary>
    /// Исключение при попытке изменить неизменяемые системные объекты.
    /// </summary>
    /// <remarks>Status: 412</remarks>
    public sealed class EntityChangeRestrictException<T> : PreconditionFailedException
    {
        public EntityChangeRestrictException(object key, string? message) : base(message)
        {
            Data["Key"] = key;
            Data["Type"] = typeof(T).Name;
        }
    }
}