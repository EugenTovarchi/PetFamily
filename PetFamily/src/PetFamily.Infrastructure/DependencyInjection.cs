using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using PetFamily.Application.Database;
using PetFamily.Application.FileProvider;
using PetFamily.Application.MessageQueue;
using PetFamily.Infrastructure.BackgroundServices;
using PetFamily.Infrastructure.FileServices;
using PetFamily.Infrastructure.MessageQueues;
using PetFamily.Infrastructure.Options;
using PetFamily.Infrastructure.Providers;
using PetFamily.Infrastructure.Repositories;

namespace PetFamily.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(); 
        services.AddScoped<IVolunteersRepository, VolunteersRepository>();
        services.AddScoped<ISpeciesRepository, SpeciesRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddHostedService<FilesCleanerBackgroundService>();
        services.AddScoped<IFilesCleanerService, PhotosCleanerService>(); 
        //Определяем сколько будет каналов (т.к. сингл - дублируем в зависимости от желаемого ко-ва каналов) и под разные типы данных<> 
        services.AddSingleton<IMessageQueue<IEnumerable<PhotoMainData>>, InMemoryMessageQueue<IEnumerable<PhotoMainData>>>();
        services.AddMinio(configuration);

        return services;
    }

    private static IServiceCollection AddMinio(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioOptions>(configuration.GetSection(MinioOptions.MINIO));

        services.AddMinio(options =>
        {
            var minioOptions = configuration.GetSection(MinioOptions.MINIO)
            .Get<MinioOptions>() ?? throw new ApplicationException("Missing minio configuration");

            options.WithEndpoint(minioOptions.Endpoint);
            options.WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey);
            options.WithSSL(minioOptions.WithSsl);
        });
        services.AddScoped<IFileProvider, MinioProvider>();

        return services;
    }
}