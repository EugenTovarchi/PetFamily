using Microsoft.Extensions.DependencyInjection;

namespace PetFamily.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgresInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>();

        return services;
    }
}