using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using PetFamily.Application.Database;
using PetFamily.Application.Volunteers.MovePetPosition;
using PetFamily.Contracts.Commands.Volunteers;
using PetFamily.Contracts.Requests.Volunteers;
using PetFamily.Domain.PetManagment.AggregateRoot;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.Extensions;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.UnitTests;

public  class MovePetPositionTest
{
    private readonly Mock<IVolunteersRepository> _volunteerRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IValidator<MovePetPositionCommand>> _validatorMock;
    private readonly Mock<ILogger<MovePetPositionHandler>> _loggerMock;
    public MovePetPositionTest()
    {
        _volunteerRepositoryMock = new Mock<IVolunteersRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validatorMock = new Mock<IValidator<MovePetPositionCommand>>();
        _loggerMock = new Mock<ILogger<MovePetPositionHandler>>();
    }
    [Fact]
    public async Task Handle_Should_Move_PetPosition()
    {

        const int petsCount = 5;
        const int POSITION_NUMBER = 3;

        var volunteer = CreateTestVolunteer();
        var pets = Enumerable.Range(1, petsCount).Select(_ => CreateTestPet());

        foreach (var pet in pets)
        {
            volunteer.AddPet(pet);
        }

        var firdPosition = Position.Create(3).Value;

        var firstPet = volunteer.Pets.ElementAt(0);
        var secondPet = volunteer.Pets.ElementAt(1);
        var thirdPet = volunteer.Pets.ElementAt(2);
        var fourthPet = volunteer.Pets.ElementAt(3);
        var fifthPet = volunteer.Pets.ElementAt(4);

        var ct = new CancellationTokenSource().Token;
        MovePetPositionRequest request = new (POSITION_NUMBER);
        MovePetPositionCommand command = new(volunteer.Id, fifthPet.Id, request);

        _volunteerRepositoryMock.Setup(r => r.GetById(volunteer.Id, ct)).ReturnsAsync(volunteer);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(ct)).Returns(Task.CompletedTask);

        _validatorMock.Setup(v => v.ValidateAsync(command, ct)).ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var handler = new MovePetPositionHandler(
            _volunteerRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);

        //act
        var result = await handler.Handle(command, ct);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(POSITION_NUMBER);
        firstPet.Position.Should().Be(Position.Create(1).Value);
        secondPet.Position.Should().Be(Position.Create(2).Value);
        thirdPet.Position.Should().Be(Position.Create(4).Value);
        fourthPet.Position.Should().Be(Position.Create(5).Value);
        fifthPet.Position.Should().Be(Position.Create(3).Value);
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

