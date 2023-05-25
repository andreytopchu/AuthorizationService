using System;
using Identity.Application.Abstractions.Models.Command.Role;

namespace Identity.Models.Requests.Role;

public class UpdateRoleRequest : BaseRoleRequest, IUpdateRoleCommand
{
    public Guid Id { get; init; }
}