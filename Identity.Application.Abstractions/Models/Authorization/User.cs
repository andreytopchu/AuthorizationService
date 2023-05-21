namespace Identity.Application.Abstractions.Models.Authorization;

public class User
{
    public string Subject { get; init; }
    public string Name { get; init; }
    public bool IsActive { get; init; }
    public SimpleClaim[] Claims { get; init; }
}