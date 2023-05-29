using Identity.Domain.Entities;
using Identity.Domain.Exceptions;

namespace Identity.Application.Extensions;

internal static class CheckPolicyIsNotFullAccessExtension
{
    internal static void ThrowIfPolicyIsFullAccess(this string policyName)
    {
        if (policyName == "fullAccess")
        {
            throw new EntityChangeRestrictException<Policy>(0, "You can't create or change an policy fullAccess");
        }
    }
}