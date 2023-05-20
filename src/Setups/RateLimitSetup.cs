using AspNetCoreRateLimit;

namespace WebApi.Setups;

public static class RateLimitSetup
{
    public static void AddCustomRateLimitiong(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddInMemoryRateLimiting();

        services.Configure<IpRateLimitOptions>(options =>
        {
            options.EnableEndpointRateLimiting = true;
            options.StackBlockedRequests = false;
            options.HttpStatusCode = 429;
            options.RealIpHeader = "X-Real-IP";
            options.GeneralRules = new List<RateLimitRule>
                {
                    new RateLimitRule
                    {
                        Endpoint ="*",
                        Period = "1s",
                        Limit = 2    
                        //Period = "10s",
                        //Limit =2
                    }
                };
        });


    }

}

