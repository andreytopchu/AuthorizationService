namespace Identity.Application.Abstractions.Models;

public class Credential
{
    public string Username { get; init; }
    public string Password { get; init; }
    public string ClientId { get; init; }
}