using System.Text.Json.Serialization;
using Identity.Application.Abstractions.Models.Command.User;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Identity.Models.Requests.User;

public class AddUserRequest : BaseUserRequest, IAddUserCommand
{
    public string Password { get; }

    [JsonIgnore]
    [ValidateNever]
    public IIdentityUriCommand? IdentityUri { get; set; }
}