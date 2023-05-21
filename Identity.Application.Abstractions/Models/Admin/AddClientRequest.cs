namespace Identity.Application.Abstractions.Models.Admin;

public class AddClientRequest : UpdateClientRequest
{
    public string ClientName { get; }
    public string ClientId { get; }
}