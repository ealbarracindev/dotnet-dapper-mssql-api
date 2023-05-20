using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json.Serialization;
using WebApi.Extensions;
using WebApi.Helpers;
using WebApi.Repositories;
using WebApi.Services;
using WebApi.Setups;

var builder = WebApplication.CreateBuilder(args);

// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;
 
    services.AddCors();
    // Controllers
    services.AddControllers().AddJsonOptions(x =>
    {
        // serialize enums as strings in api responses (e.g. Role)
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // ignore omitted parameters on models to enable optional params (e.g. User update)
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    // HealthChecks (API, Dapper, MSSQLDB, Redis)
    builder.Services.AddCustomHealthChecks(builder.Configuration);

    //Rate limit
    builder.Services.AddCustomRateLimitiong();

    // Swagger
    builder.Services.AddSwaggerSetup();
    // Persistence layer setup
    //builder.Services.AddPersistenceSetup(builder.Configuration);
    // Request response compression
    builder.Services.AddCompressionSetup();

    // configure strongly typed settings object
    services.Configure<DbSettings>(builder.Configuration.GetSection("DbSettings"));

    // configure DI for application services
    services.AddSingleton<DataContext>();
    //services.AddScoped<DapperWrap>();
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IUserService, UserService>();

    // Add serilog
    if (builder.Environment.EnvironmentName != "Testing")
    {
        builder.Host.UseLoggingSetup(builder.Configuration);
    }
}


var app = builder.Build();

// ensure database and tables exist
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    await context.Init();
}

// configure HTTP request pipeline
{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    app.UseIpRateLimiting();

    app.UseSwaggerSetup();

    // Configure the HTTP request pipeline gzip.
    app.UseResponseCompression();

    app.UseHealthChecks("/health", new HealthCheckOptions
    {
        AllowCachingResponses = false,
        ResponseWriter = HealthCheckExtensions.WriteResponse
    });

    app.MapControllers();
}

//app.Run("http://localhost:4000");
app.Run();