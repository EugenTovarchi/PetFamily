using FluentValidation;
using PetFamily.Contracts.Requests;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Volunteers;
using Shared;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerService
{
    private readonly IVolunteersRepository _repository;
    private readonly IValidator<CreateVolunteerRequest>  _validator;
    public CreateVolunteerService(IVolunteersRepository repository, IValidator<CreateVolunteerRequest> validator)
    {
        _repository = repository; 
        _validator = validator;
    }
    public async Task <Result<Guid>> Handle(CreateVolunteerCommand command, CancellationToken cancellationToken = default)
    {
        if (command is null)
            return Errors.General.ValueIsInvalid("request");

        var validationResult = await _validator.ValidateAsync(command.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Errors.Validation.RecordIsInvalid("createVolunteerRequest");
        }

        var existVolunteer = await _repository.GetByName(
            command.Request.FirstName,
            command.Request.LastName,
            command.Request.MiddleName,
            cancellationToken);

        if (existVolunteer.IsSuccess)
            return Errors.General.Duplicate("existVolunteer");


        var fullName = command.Request.MiddleName is null
        ? FullName.Create(command.Request.FirstName, command.Request.LastName)
        : FullName.CreateWithMiddle(command.Request.FirstName, command.Request.LastName, command.Request.MiddleName);

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
