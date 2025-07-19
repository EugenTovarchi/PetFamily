using PetFamily.Domain.Shared;

namespace PetFamily.Domain.Pets;

public record PetAddress
{
    private const int MAX_LENGTH = 100;
    public string City { get; }
    public string Street { get; }   
    public uint  House { get; }
    public uint Flat { get; }


    private PetAddress(string city, string street, uint house, uint flat)
    {
        City = city.Trim();
        Street = street.Trim();
        House = house;
        Flat = flat;
    }

    public static Result<PetAddress> Create(
        string city,
        string street,
        uint house,
        uint flat)
    {
        if (string.IsNullOrWhiteSpace(city) && city.Length > MAX_LENGTH)
            return "Город не может быть пустым и превышать 100 символов";

        if (string.IsNullOrWhiteSpace(street) && street.Length > MAX_LENGTH)
            return "Улица не может быть пустой и превышать 100 символов";

        if (house == 0)
            return "Номер дома должен быть положительным числом";

        return new PetAddress(
            city.Trim(),
            street.Trim(),
            house,
            flat);
    }
}
