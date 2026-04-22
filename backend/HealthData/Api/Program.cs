using Api.ExceptionHandling;
using Common.Versioning;
using DataAccess;
using DataAccess.Context;
using DataAccess.Interfaces;
using Logic;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile(
                        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                        optional: true,
                        reloadOnChange: true)
                    .Build())
                .CreateLogger();

            try
            {
                Log.Information("Starting HealthData API");

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog();
                ConfigureServices(builder);

                var app = builder.Build();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseCors("FrontendPolicy");
                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            var versionManifestPath = Path.Combine(AppContext.BaseDirectory, "versions.json");

            builder.Services.AddSingleton<IVersionManifestProvider>(
                _ => new VersionManifestProvider(versionManifestPath));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            builder.Services.AddScoped<GlobalExceptionFilter>();

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IImportBatchDataAccess, ImportBatchDataAccess>();
            builder.Services.AddScoped<IActivityDayDataAccess, ActivityDayDataAccess>();
            builder.Services.AddScoped<ISleepSessionDataAccess, SleepSessionDataAccess>();
            builder.Services.AddScoped<IHeartRateDayDataAccess, HeartRateDayDataAccess>();

            builder.Services.AddScoped<IImportBatchLogic, ImportBatchLogic>();
            builder.Services.AddScoped<IActivityDayReadLogic, ActivityDayReadLogic>();
            builder.Services.AddScoped<IHomepageDashboardLogic, HomepageDashboardLogic>();
            builder.Services.AddScoped<ISleepSessionReadLogic, SleepSessionReadLogic>();
            builder.Services.AddScoped<IHeartRateDayReadLogic, HeartRateDayReadLogic>();
        }
    }
}

