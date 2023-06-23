namespace Identity.Domain.Exceptions;

/// <summary>
/// Клиент не найден
/// Status:404
/// </summary>
public sealed class ApiResourceNotFoundException : NotFoundException
{
    public ApiResourceNotFoundException(string apiResourceName)
    {
        Data[nameof(apiResourceName)] = apiResourceName;
    }
}