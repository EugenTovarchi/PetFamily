using Microsoft.Extensions.Logging;
using PetFamily.Contracts.Requests;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Application.Volunteers.UpdateMainInfoCommand;

public class UpdateMainInfoHandler
{
    private readonly IVolunteersRepository _repository;
    private readonly ILogger<UpdateMainInfoHandler> _logger;
    public UpdateMainInfoHandler(IVolunteersRepository repository,
        ILogger<UpdateMainInfoHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(UpdateMainInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
            return Errors.General.ValueIsInvalid("request");

        var volunteer = await _repository.GetById(
           request.Id,
            cancellationToken);

        if (!volunteer.IsSuccess)
        {
            _logger.LogWarning("Волонтёр {request.Id} не существует!", request.Id);

            return Errors.Volunteer.NotFound("volunteer");
        }

        var fullName = request.Dto.FullName.MiddleName is null
        ? FullName.Create(
            request.Dto.FullName.FirstName,
            request.Dto.FullName.LastName)
        : FullName.CreateWithMiddle(
            request.Dto.FullName.FirstName,
            request.Dto.FullName.LastName,
            request.Dto.FullName.MiddleName);

        var volunteerId = VolunteerId.Create(request.Id);

        var phone = Phone.Create(request.Dto.Phone).Value;

        var email = Email.Create(request.Dto.Email).Value;

        var volunteerInfo = request.Dto.VolunteerInfo;

        var experienceYears = request.Dto.ExperienceYears;


        //Не стал добавлять т.к. это не обязательные вещи при создании Волонтера( у него может не быть сразу реквизитов или соц сетей)

        //var socialMedias = command.Request.VolunteerSocialMediaDtos
        //    .Select(sm => VolunteerSocialMedia.Create(sm.Title, sm.Url)).ToList();

        //var requisites = command.Request.RequisitesDtos
        //    .Select(sm => Requisites.Create(sm.Title, sm.Instruction, sm.Value)).ToList();

        volunteer.Value.UpdateMainInfo(
            fullName.Value,
            email,
            phone,
            volunteerInfo,
            experienceYears
        );

        await _repository.Save(volunteer.Value, cancellationToken);
        _logger.LogInformation("Данные волонтёра : {volunteerId} обновлены ", volunteerId);

        return volunteer.Value.Id.Value;
    }
}
