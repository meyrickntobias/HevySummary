using dotenv.net;
using HevySummary.Services;

namespace HevySummary;

public class Program
{
    public static void Main(string[] args)
    {
        DotEnv.Load();
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();

        builder.Services.AddHttpClient();

        builder.Services.AddTransient<IHevyApiService, HevyApiService>();

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