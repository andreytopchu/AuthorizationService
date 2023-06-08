using Identity.Application.Abstractions.Models.Command.Client;

namespace Identity.Models.Requests.Client;

public class UpdateClientRequest : BaseClientRequest, IUpdateClientCommand
{
    public long Id { get; init; }
}