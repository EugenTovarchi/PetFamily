using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Volunteers;
using PetFamily.Infrastructure.Repositories;

namespace PetFamily.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(); 
        services.AddScoped<IVolunteersRepository, VolunteersRepository>();

        return services;
    }
}