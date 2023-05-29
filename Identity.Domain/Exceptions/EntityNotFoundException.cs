namespace Identity.Domain.Exceptions;

/// <summary>
/// Сущность не найдена
/// </summary>
public class EntityNotFoundException : NotFoundException
{
    public EntityNotFoundException(object key, string type)
    {
        // ReSharper disable VirtualMemberCallInConstructor
        Data["Key"] = key;
        Data["Type"] = type;
    }
}