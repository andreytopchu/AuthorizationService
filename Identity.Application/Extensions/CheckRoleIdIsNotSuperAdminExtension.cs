using Identity.Application.Abstractions.Models.Query.Role;
using Identity.Domain.Constants;
using Identity.Domain.Exceptions;

namespace Identity.Application.Extensions;

internal static class CheckRoleIdIsNotSuperAdminExtension
{
    internal static void ThrowIfRoleIdIsSuperAdmin(this Guid roleId)
    {
        if (roleId == RoleConstantId.SuperAdminRoleId)
            throw new EntityChangeRestrictException<RoleInfo>(roleId, "User with SuperAdmin role is read-only");
    }
}