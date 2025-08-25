using PetFamily.Domain.PetManagment.AggregateRoot;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerService
{
    private readonly IVolunteersRepository _repository;
    public CreateVolunteerService(IVolunteersRepository repository) => _repository = repository;
    public async Task <Result<Guid>> Handle(CreateVolunteerCommand command, CancellationToken cancellationToken = default)
    {
        if (command is null)
            return Errors.General.ValueIsInvalid("request");

        var existVolunteer = await _repository.GetByName(
            command.Request.FullName.FirstName,
            command.Request.FullName.LastName,
            command.Request.FullName.MiddleName,
            cancellationToken);

        if (existVolunteer.IsSuccess)
            return Errors.General.Duplicate("existVolunteer");


        var fullName = command.Request.FullName.FirstName is null
        ? FullName.Create(
        command.Request.FullName.FirstName!,
        command.Request.FullName.LastName)
        : FullName.CreateWithMiddle(
            command.Request.FullName.FirstName,
            command.Request.FullName.FirstName,
            command.Request.FullName.FirstName);

        var volunteerId = VolunteerId.NewVolunteerId();

        var phone = Phone.Create(command.Request.Phone).Value;
        
        var email = Email.Create(command.Request.Email).Value;

        //Не стал добавлять т.к. это не обязательные вещи при создании Волонтера( у него может не быть сразу реквизитов или соц сетей)

        //var socialMedias = command.Request.VolunteerSocialMediaDtos
        //    .Select(sm => VolunteerSocialMedia.Create(sm.Title, sm.Url)).ToList();

        //var requisites = command.Request.RequisitesDtos
        //    .Select(sm => Requisites.Create(sm.Title, sm.Instruction, sm.Value)).ToList();

        var volunteer = new Volunteer
        (
            volunteerId,
            fullName.Value,
            email,
            phone,
            command.Request.VolunteerInfo,
            command.Request.ExperienceYears
        );

        await _repository.Add(volunteer, cancellationToken);
        return volunteer.Id.Value;
    }
}
