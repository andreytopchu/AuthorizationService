namespace Identity.Application.Abstractions.Models.Command.Client;

public interface IDeleteClientCommand
{
    public long Id { get; init; }
}