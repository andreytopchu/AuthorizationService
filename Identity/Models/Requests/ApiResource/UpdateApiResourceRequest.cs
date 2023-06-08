using Identity.Application.Abstractions.Models.Command.ApiResource;

namespace Identity.Models.Requests.ApiResource;

public class UpdateApiResourceRequest : BaseApiResourceRequest, IUpdateApiResourceCommand
{
    public int Id { get; init; }
}