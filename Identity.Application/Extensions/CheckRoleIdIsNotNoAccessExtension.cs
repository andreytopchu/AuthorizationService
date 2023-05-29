using Identity.Application.Abstractions.Models.Query.Role;
using Identity.Domain.Constants;
using Identity.Domain.Exceptions;

namespace Identity.Application.Extensions;

internal static class CheckRoleIdIsNotNoAccessExtension
{
    internal static void ThrowIfRoleIdIsNoAccess(this Guid roleId)
    {
        if (roleId == RoleConstantId.NoAccessRoleId)
            throw new EntityChangeRestrictException<RoleInfo>(roleId, "NoAccess role is read-only");
    }
}