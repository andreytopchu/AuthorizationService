using Identity.Application.Abstractions.Models.Command.Client;

namespace Identity.Models.Requests.Client;

public class DeleteClientRequest: IDeleteClientCommand
{
    public long Id { get; init; }
}