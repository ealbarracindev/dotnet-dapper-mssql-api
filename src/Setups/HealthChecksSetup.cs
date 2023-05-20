using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebApi.Helpers;

namespace WebApi.Setups;

public static class HealthChecksSetup
{
    private static readonly string[] DatabaseTags = new[] { "database" };

    #region Config Health Check
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration) {
            var _dbSettings  = configuration.GetSection("DbSettings").Get<DbSettings>();
        var connectionString = $"Server={_dbSettings.Server}; Database={_dbSettings.Database}; User Id={_dbSettings.UserId}; Password={_dbSettings.Password}; TrustServerCertificate=True";
        //var cnn = configuration.GetSection("DbSettings");
        services.AddHealthChecks()
                    //.AddCheck("self", () => HealthCheckResult.Healthy())
                    .AddCheck<GCInfoHealthCheck>("api")
                    .AddSqlServer(connectionString, tags: DatabaseTags);
                    //.AddRedis("redis", tags: new[] { "dependencies" })
                    

        //public static void AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
        //{
        //    var connectionOptions = configuration.GetOptions<ConnectionOptions>(ConnectionOptions.ConfigSectionPath);

        //    var healthCheckBuilder = services
        //         .AddHealthChecks()
        //         .AddDbContextCheck<TiendaContext>(tags: DatabaseTags);


        //    if (!connectionOptions.CacheConnection.IsInMemoryCache())
        //        healthCheckBuilder.AddRedis(connectionOptions.CacheConnection);
        //}
        return services;
    }
    #endregion
}

