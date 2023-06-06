namespace Identity.Domain.Exceptions;

/// <summary>
/// Клиент не найден
/// Status:404
/// </summary>
public sealed class ClientNotFoundException : NotFoundException
{
    public ClientNotFoundException(string clientId)
    {
        Data[nameof(clientId)] = clientId;
    }
}