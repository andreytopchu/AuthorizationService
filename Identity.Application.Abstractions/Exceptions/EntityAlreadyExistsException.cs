namespace Identity.Application.Abstractions.Exceptions
{
    public sealed class EntityAlreadyExistsException<T> : AlreadyExistException
    {
        /// <summary>
        /// Эксепшен указывающий на то, что объект уже существует
        /// </summary>
        /// <remarks>Status: 409</remarks>
        public EntityAlreadyExistsException(object key)
        {
            Data["Key"] = key;
            Data["Type"] = typeof(T).Name;
        }
    }

    public class EntityAlreadyExistsException : AlreadyExistException
    {
        /// <summary>
        /// Эксепшен указывающий на то, что объект уже существует
        /// </summary>
        /// <remarks>Status: 409</remarks>
        public EntityAlreadyExistsException(string message, Exception? ex) : base(message, ex)
        {
        }
    }
}