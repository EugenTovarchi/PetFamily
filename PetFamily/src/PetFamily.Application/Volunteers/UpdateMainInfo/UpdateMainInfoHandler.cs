using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Contracts.Requests;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Application.Volunteers.UpdateMainInfoCommand;

public class UpdateMainInfoHandler
{
    private readonly IVolunteersRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateMainInfoHandler> _logger;
    public UpdateMainInfoHandler(IVolunteersRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateMainInfoHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
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

        volunteer.Value.UpdateMainInfo(
            fullName.Value,
            email,
            phone,
            volunteerInfo,
            experienceYears
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Данные волонтёра : {volunteerId} обновлены ", volunteerId);

        return volunteer.Value.Id.Value;
    }
}
