using dotenv.net;
using HevySummary.Services;
using StackExchange.Redis;

namespace HevySummary;

public class Program
{
    public static void Main(string[] args)
    {
        DotEnv.Load();
        var builder = WebApplication.CreateBuilder(args);
        
        ConfigureServices(builder.Services, builder.Configuration);
        
        var app = builder.Build();

        app.UseHttpsRedirection();
        
        app.MapControllers();
        app.UseCors(policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        
        services.AddLogging();

        services.AddHttpClient();
        
        services.AddTransient<IMuscleGroupService, MuscleGroupService>();

        services.AddTransient<IHevyApiService, HevyApiService>();
        
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis") 
            ?? throw new Exception("Redis connection string isn't configured.")));

        services.AddSingleton<ICacheService, RedisCacheService>();

        services.AddTransient<TimeProvider>(_ => TimeProvider.System);
    }
}