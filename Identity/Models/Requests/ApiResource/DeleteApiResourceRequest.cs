using Identity.Application.Abstractions.Models.Command.ApiResource;

namespace Identity.Models.Requests.ApiResource;

public class DeleteApiResourceRequest: IDeleteApiResourceCommand
{
    public string ApiResourceName { get; init; }
}