using Identity.Application.Abstractions.Models.Command.Client;
using Identity.Models.Requests.ApiResource;

namespace Identity.Models.Requests.Client;

public class AddClientRequest : BaseApiResourceRequest, IAddClientCommand
{
}