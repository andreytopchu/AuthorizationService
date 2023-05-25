namespace Identity.Domain.Exceptions
{
    public sealed class EntityInUseException<T> : AlreadyExistException
    {
        /// <summary>
        /// Эксепшен указывающий на то, что объект используется
        /// и требуемое действие над ним (обычно удаление) - невозможно
        /// </summary>
        /// <remarks>Status: 409</remarks>
        public EntityInUseException(object key)
        {
            Data["Key"] = key;
            Data["Type"] = typeof(T).Name;
        }
    }
}