using Microsoft.OpenApi.Models;
using PetFamily.API.Middlewares;
using PetFamily.Infrastructure;
using Serilog;
using Serilog.Events;

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
            .WriteTo.Seq(
                    serverUrl: builder.Configuration.GetConnectionString("Seq") ?? throw new ArgumentNullException("Seq"),
                    apiKey: builder.Configuration["Seq:ApiKey"], 
                    restrictedToMinimumLevel: LogEventLevel.Verbose)
                        .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
            .CreateLogger();


        builder.Services.AddControllers();
        builder.Services.AddSerilog();
        builder.Host.UseSerilog();
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

        app.UseExceptionMiddleware();

        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
