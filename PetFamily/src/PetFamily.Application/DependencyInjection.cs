using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Application.Volunteers.DeleteCommand;
using PetFamily.Application.Volunteers.UpdateMainInfoCommand;
using PetFamily.Application.Volunteers.UpdateRequisitesCommand;
using PetFamily.Application.Volunteers.UpdateSocialMediasCommand;

namespace PetFamily.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateVolunteerHandler>();
        services.AddScoped<UpdateMainInfoHandler>();
        services.AddScoped<DeleteVolunteerHandler>();
        services.AddScoped<UpdateSocialMediasHandler>();
        services.AddScoped<UpdateRequisitesHandler>(); 


        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}