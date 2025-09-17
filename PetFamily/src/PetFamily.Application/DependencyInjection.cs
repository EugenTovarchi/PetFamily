using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Species.AddBreed;
using PetFamily.Application.Species.CreateSpecies;
using PetFamily.Application.Species.DeleteBreed;
using PetFamily.Application.Species.DeleteSpecies;
using PetFamily.Application.Volunteers.AddPet;
using PetFamily.Application.Volunteers.Create;
using PetFamily.Application.Volunteers.DeletePetPhotos;
using PetFamily.Application.Volunteers.GetPetPhotos;
using PetFamily.Application.Volunteers.HardDelete;
using PetFamily.Application.Volunteers.MovePetPosition;
using PetFamily.Application.Volunteers.Restore;
using PetFamily.Application.Volunteers.SoftDelete;
using PetFamily.Application.Volunteers.UpdateMainInfo;
using PetFamily.Application.Volunteers.UpdateRequisites;
using PetFamily.Application.Volunteers.UpdateSocialMedias;
using PetFamily.Application.Volunteers.UploadPetPhotos;

namespace PetFamily.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateVolunteerHandler>();
        services.AddScoped<UpdateMainInfoHandler>();
        services.AddScoped<SoftDeleteVolunteerHandler>();
        services.AddScoped<HardDeleteVolunteerHandler>();
        services.AddScoped<UpdateSocialMediasHandler>();
        services.AddScoped<UpdateRequisitesHandler>(); 
        services.AddScoped<RestoreDeletedVolunteerHandler>();
        services.AddScoped<AddPetHandler>();
        services.AddScoped<UploadPetPhotosHandler>();
        services.AddScoped<CreateSpeciesHandler>();
        services.AddScoped<DeleteSpeciesHandler>();
        services.AddScoped<DeleteBreedHandler>();
        services.AddScoped<AddBreedHandler>();
        services.AddScoped<DeletePetPhotosHandler>();
        services.AddScoped<GetPetPhotosHandler>();
        services.AddScoped<MovePetPositionHandler>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}