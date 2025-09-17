using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using PetFamily.Application.Database;
using PetFamily.Application.FileProvider;
using PetFamily.Application.Volunteers.UploadPetPhotos;
using PetFamily.Contracts.Commands.Volunteers;
using PetFamily.Contracts.Dtos.FileProviderData;
using PetFamily.Domain.PetManagment.AggregateRoot;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.Extensions;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.UnitTests;

public class UploadPetPhotosTests
{
    [Fact]
    public async Task Handle_Shoul_UploadFiles_To_Pet()
    {
        //arrange
        var ct = new CancellationTokenSource().Token;
        var volunteer = CreateTestVolunteer();
        var pet = CreateTestPet();
        volunteer.AddPet(pet);  

        var stream = new MemoryStream();
        var fileTestName = "test.jpg";

        List<PhotoPath> photoPaths  = [PhotoPath.Create(fileTestName).Value, PhotoPath.Create(fileTestName).Value];
        var uploadFileDto = new UploadFileDto(stream, fileTestName);

        List<UploadFileDto> files = [uploadFileDto, uploadFileDto]; 

        UploadPetPhotosCommand command = new (volunteer.Id, pet.Id, files);

        var volunteerRepositoryMock = new Mock<IVolunteersRepository>();
        volunteerRepositoryMock.Setup(r => r.GetById(volunteer.Id, ct)).ReturnsAsync(volunteer);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.SaveChangesAsync(ct)).Returns(Task.CompletedTask);

        var loggerMock = new Mock<ILogger<UploadPetPhotosHandler>>();

        //данные на возврат смотрим в методе IFileProvider
        var fileProviderMock = new Mock<IFileProvider>();   
        fileProviderMock.Setup(f=>f.UploadFiles(It.IsAny<List<PhotoData>>(), ct)).ReturnsAsync(photoPaths);

        //вкладываем положительный результат валидации
        var fluentValidatorMock = new Mock<IValidator<UploadPetPhotosCommand>>();
        fluentValidatorMock.Setup(v => v.ValidateAsync(command,ct)).ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var handler = new UploadPetPhotosHandler(
            fileProviderMock.Object,
            volunteerRepositoryMock.Object,
            unitOfWorkMock.Object,
            fluentValidatorMock.Object,
            loggerMock.Object);

        //act
        var result = await handler.Handle(command, ct);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(pet.Id.Value);
        pet.Photos.Should().HaveCount(2);
        pet.Photos.Should().OnlyContain(photo => photo.PathToStorage.Path.Contains(fileTestName));
    }

    private Pet CreateTestPet()
    {
        var petId = PetId.NewPetId();
        var petName = "test";
        var petDescription = "test";
        var petHealthInfo = "test";
        var petVaccinated = true;
        var petHeight = 100;
        var petWeight = 100;
        var dateTime = DateTime.UtcNow;
        var petStatus = EnumExtension.ParsePetStatus("LookingHome").Value;
        var petColor = EnumExtension.ParsePetColor("Белый").Value;
        var address = Address.Create("City", "street", 5).Value;
        var speciesId = Guid.NewGuid();
        var breedId = Guid.NewGuid();
        var petType = PetType.Create(speciesId, breedId).Value;

        var pet = new Pet(
            petId, petName,
            petDescription,
            petHealthInfo,
            address,
            petVaccinated,
            petHeight,
            petWeight,
            petType,
            dateTime,
            null,
            petColor,
            petStatus);

        return pet;
    }

    private Volunteer CreateTestVolunteer()
    {
        var volunteerId = VolunteerId.NewVolunteerId();
        var volunteerName = FullName.Create("Test", "Test").Value;
        var email = Email.Create("test@mail.com");
        var phone = Phone.Create("123456788");
        var volunteerInfo = "info";
        var expYears = 1;

        var volunteer = new Volunteer(
            volunteerId,
            volunteerName,
            email.Value,
            phone.Value,
            volunteerInfo,
            expYears);

        return volunteer;
    }
}
