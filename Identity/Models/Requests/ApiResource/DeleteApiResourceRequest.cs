using Identity.Application.Abstractions.Models.Command.ApiResource;

namespace Identity.Models.Requests.ApiResource;

public class DeleteApiResourceRequest: IDeleteApiResourceCommand
{
    public long ApiResourceId { get; init; }
}