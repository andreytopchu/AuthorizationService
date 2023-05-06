namespace Identity.Application.Abstractions.Models;

public class Subject
{
    public string Sub { get; init; } = null!;
    public List<SimpleClaim> Claims { get; init; } = new ();
}