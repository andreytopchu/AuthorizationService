namespace Identity.Domain.Exceptions;

/// <summary>
/// Исключение при невозможности обнаружить объект по переданным параметрам.
/// </summary>
/// <remarks>Status: 404</remarks>
public sealed class EntityNotFoundException<T> : EntityNotFoundException
{
    public EntityNotFoundException(object key) : base(key, typeof(T).Name)
    {
    }
}