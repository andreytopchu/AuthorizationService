namespace Identity.Application.Abstractions.Models.Admin;

public class AddUserRequest : UpdateUserRequest
{
    public string Password { get; }
}