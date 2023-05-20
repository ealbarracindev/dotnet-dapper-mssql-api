using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebApi.Helpers;

namespace WebApi.Setups;

public static class GCInfoHealthCheckSetup
{
    public static IHealthChecksBuilder AddGCInfoCheck(this IHealthChecksBuilder builder)
        => builder.AddCheck<GCInfoHealthCheck>("GCInfo", HealthStatus.Degraded, tags: new[] { "memory" });
}
