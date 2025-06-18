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
        
        builder.Services.AddControllers();
        
        builder.Services.AddLogging();

        builder.Services.AddHttpClient();
        
        builder.Services.AddTransient<IMuscleGroupService, MuscleGroupService>();

        builder.Services.AddTransient<IHevyApiService, HevyApiService>();
        
        builder.Services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis") ?? throw new Exception("Redis connection string isn't configured.")));

        builder.Services.AddSingleton<ICacheService, RedisCacheService>();
        
        var app = builder.Build();

        app.UseHttpsRedirection();
        
        app.MapControllers();
        app.UseCors(policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });

        // app.UseAuthorization();

        app.Run();
    }
}