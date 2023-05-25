using System;
using Identity.Application.Abstractions.Models.Command.Role;

namespace Identity.Models.Requests.Role;

public class DeleteRoleRequest : IDeleteRoleCommand
{
    public Guid RoleId { get; set; }
}