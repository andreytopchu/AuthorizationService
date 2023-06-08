namespace Identity.Domain.Exceptions;

/// <summary>
/// Клиент не найден
/// Status:404
/// </summary>
public sealed class ApiResourceNotFoundException : NotFoundException
{
    public ApiResourceNotFoundException(int apiResourceId)
    {
        Data[nameof(apiResourceId)] = apiResourceId;
    }
}