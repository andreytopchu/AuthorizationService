using System.Diagnostics;

namespace Identity.Application.Abstractions.Extensions;

public static class ActivityIdGuidExtension
{
    public static Guid GetTraceId(this Activity? activity)
    {
        if (activity == null)
            throw new InvalidOperationException("activity is not started");

        return Guid.Parse(activity.TraceId.ToHexString());
    }
}