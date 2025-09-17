using FluentAssertions;
using CSharpFunctionalExtensions;
using PetFamily.Domain.PetManagment.AggregateRoot;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.Extensions;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.UnitTests
{
    public class VolunteerTests
    {
        [Fact]
        public void Move_Pet_Should_Move_Other_Pets_Forward_When_New_Position_Is_First()
        {
            //arrange
            const int petsCount = 5;

            var volunteer = CreateTestVolunteer();
            var pets = Enumerable.Range(1, petsCount).Select(_ => CreateTestPet());

            foreach (var pet in pets)
            {
                volunteer.AddPet(pet);
            }

            var firstPosition = Position.Create(1).Value;

            var firstPet = volunteer.Pets.ElementAt(0);
            var secondPet = volunteer.Pets.ElementAt(1);
            var thirdPet = volunteer.Pets.ElementAt(2);
            var fourthPet = volunteer.Pets.ElementAt(3);
            var fifthPet = volunteer.Pets.ElementAt(4);

            //act

            var result = volunteer.MovePet(thirdPet, firstPosition);

            //assert
            result.IsSuccess.Should().BeTrue();
            firstPet.Position.Should().Be(Position.Create(2).Value);
            secondPet.Position.Should().Be(Position.Create(3).Value);
            thirdPet.Position.Should().Be(Position.Create(1).Value);
            fourthPet.Position.Should().Be(Position.Create(4).Value);
            fifthPet.Position.Should().Be(Position.Create(5).Value);
        }

        [Fact]
        public void Move_Pet_Should_Move_Other_Pets_Back_When_New_Position_Is_Last()
        {
            //arrange
            const int petsCount = 5;

            var volunteer = CreateTestVolunteer();
            var pets = Enumerable.Range(1, petsCount).Select(_ => CreateTestPet());

            foreach (var pet in pets)
            {
                volunteer.AddPet(pet);
            }

            var fifthPosition = Position.Create(5).Value;

            var firstPet = volunteer.Pets.ElementAt(0);
            var secondPet = volunteer.Pets.ElementAt(1);
            var thirdPet = volunteer.Pets.ElementAt(2);
            var fourthPet = volunteer.Pets.ElementAt(3);
            var fifthPet = volunteer.Pets.ElementAt(4);

            //act

            var result = volunteer.MovePet(secondPet, fifthPosition);

            //assert
            result.IsSuccess.Should().BeTrue();
            firstPet.Position.Should().Be(Position.Create(1).Value);
            secondPet.Position.Should().Be(Position.Create(5).Value);
            thirdPet.Position.Should().Be(Position.Create(2).Value);
            fourthPet.Position.Should().Be(Position.Create(3).Value);
            fifthPet.Position.Should().Be(Position.Create(4).Value);
        }

        [Fact]
        public void Move_Pet_Should_Move_Other_Pets_Forward_When_New_Position_Is_Lower()
        {
            //arrange
            const int petsCount = 5;

            var volunteer = CreateTestVolunteer();
            var pets = Enumerable.Range(1, petsCount).Select(_ => CreateTestPet());

            foreach (var pet in pets)
            {
                volunteer.AddPet(pet);
            }

            var secondPosition = Position.Create(2).Value;

            var firstPet = volunteer.Pets.ElementAt(0);
            var secondPet = volunteer.Pets.ElementAt(1);
            var thirdPet = volunteer.Pets.ElementAt(2);
            var fourthPet = volunteer.Pets.ElementAt(3);
            var fifthPet = volunteer.Pets.ElementAt(4);

            //act

            var result = volunteer.MovePet(fourthPet, secondPosition);

            //assert
            result.IsSuccess.Should().BeTrue();
            firstPet.Position.Should().Be(Position.Create(1).Value);
            secondPet.Position.Should().Be(Position.Create(3).Value);
            thirdPet.Position.Should().Be(Position.Create(4).Value);
            fourthPet.Position.Should().Be(Position.Create(2).Value);
            fifthPet.Position.Should().Be(Position.Create(5).Value);
        }

        [Fact]
        public void Move_Pet_Should_Move_Other_Pets_Back_When_New_Position_Is_Bigger()
        {
            //arrange
            const int petsCount = 5;

            var volunteer = CreateTestVolunteer();
            var pets = Enumerable.Range(1, petsCount).Select(_ => CreateTestPet());

            foreach (var pet in pets)
            {
                volunteer.AddPet(pet);
            }

            var fifthPosition = Position.Create(5).Value;

            var firstPet = volunteer.Pets.ElementAt(0);
            var secondPet = volunteer.Pets.ElementAt(1);
            var thirdPet = volunteer.Pets.ElementAt(2);
            var fourthPet = volunteer.Pets.ElementAt(3);
            var fifthPet = volunteer.Pets.ElementAt(4);

            //act

            var result = volunteer.MovePet(thirdPet, fifthPosition);

            //assert
            result.IsSuccess.Should().BeTrue();
            firstPet.Position.Should().Be(Position.Create(1).Value);
            secondPet.Position.Should().Be(Position.Create(2).Value);
            thirdPet.Position.Should().Be(Position.Create(5).Value);
            fourthPet.Position.Should().Be(Position.Create(3).Value);
            fifthPet.Position.Should().Be(Position.Create(4).Value);
        }


        [Fact]
        public void Move_Pet_Should_Not_Move_If_Pet_Already_At_New_Position()
        {
            //arrange
            const int petsCount = 5;

            var volunteer = CreateTestVolunteer();
            var pets = Enumerable.Range(1, petsCount).Select(_ => CreateTestPet());

            foreach (var pet in pets)
            {
                volunteer.AddPet(pet);
            }

            var secondPosition = Position.Create(2).Value;

            var firstPet = volunteer.Pets.ElementAt(0);
            var secondPet = volunteer.Pets.ElementAt(1);
            var thirdPet = volunteer.Pets.ElementAt(2);
            var fourthPet = volunteer.Pets.ElementAt(3);
            var fifthPet = volunteer.Pets.ElementAt(4);

            //act
            var result = volunteer.MovePet(secondPet, secondPosition);

            //assert
            result.IsSuccess.Should().BeTrue();
            firstPet.Position.Should().Be(Position.Create(1).Value);
            secondPet.Position.Should().Be(Position.Create(2).Value);
            thirdPet.Position.Should().Be(Position.Create(3).Value);
            fourthPet.Position.Should().Be(Position.Create(4).Value);
            fifthPet.Position.Should().Be(Position.Create(5).Value);
        }



        [Fact]
        public void Add_PeT_With_Empty_SerialNumber_Resturn_Success_Result()
        {
            //arrage(подготовка)
            var pet = CreateTestPet();
            var volunteer = CreateTestVolunteer();

            //act(действие)
            var result = volunteer.AddPet(pet);

            //assert(результат) - утверждение
            var addPetResult = (volunteer.GetPetById(pet.Id));

            result.IsSuccess.Should().BeTrue();
            addPetResult.IsSuccess.Should().BeTrue();

            addPetResult.Value.Id.Should().Be(pet.Id);
            addPetResult.Value.Position.Should().Be(Position.First);
            
        }

        [Fact]
        public void AddPet_With_Multiple_Pets_Return_Correct_SerialNumber()
        {
            const int petsCount = 5;

            // arrange
            var volunteer = CreateTestVolunteer();
            var pets = Enumerable.Range(1, petsCount).Select(_ => CreateTestPet());

            foreach (var pet in pets)
            {
                volunteer.AddPet(pet);
            }

            var petToAdd = CreateTestPet();
            var expectedSerialNumber = Position.Create(petsCount + 1).Value; 

            // act
            var result = volunteer.AddPet(petToAdd);
            var getPetResult = volunteer.GetPetById(petToAdd.Id);

            // assert
            result.IsSuccess.Should().BeTrue();
            getPetResult.IsSuccess.Should().BeTrue();
            getPetResult.Value.Position.Should().Be(expectedSerialNumber);

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
}
