namespace Identity.Application.Abstractions.Models.Authorization;

public class Subject
{
    public string Sub { get; init; } = null!;
    public List<SimpleClaim> Claims { get; init; } = new ();
}