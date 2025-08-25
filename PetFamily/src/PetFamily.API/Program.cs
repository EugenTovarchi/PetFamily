using Microsoft.OpenApi.Models;
using PetFamily.API.Middlewares;
using PetFamily.Infrastructure;
using Serilog;

namespace PetFamily.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.WithEnvironmentName()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.Seq(builder.Configuration.GetConnectionString("Seq")
                        ?? throw new ArgumentNullException("Seq"))
            .WriteTo.File(path: Path.Combine("logs", "log-.txt"),
                        rollingInterval: RollingInterval.Day, //создается каждый день новый файл
                        retainedFileCountLimit: 7, // хранить логи за 7 дней, с 8го начнут заменяться
                        rollOnFileSizeLimit: true) //когда дойдет до лимита размера файла, создасться новый того же дня.
            .CreateLogger();

        builder.Services.AddSerilog();

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "PetFamily", Version = "v1" });
        });

        builder.Services
            .AddApplication()
            .AddInfrastructure();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PetFamily v1");
            });

            await app.ApplyMigrations();
        }
        
        app.UseSerilogRequestLogging();

        app.UseExceptionMiddleware();

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
