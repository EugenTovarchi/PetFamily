using Shared;

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
        if (string.IsNullOrWhiteSpace(city))
            return Errors.General.ValueIsEmptyOrWhiteSpace("city");

        if (city.Length > MAX_LENGTH)
            return Errors.General.ValueIsRequired("city");

        if (string.IsNullOrWhiteSpace(street))
            return Errors.General.ValueIsEmptyOrWhiteSpace("street");

        if (street.Length > MAX_LENGTH)
            return Errors.General.ValueIsRequired("street");

        if (house == 0)
            return Errors.General.ValueIsZero("house");

        return new PetAddress(
            city.Trim(),
            street.Trim(),
            house,
            flat);
    }
}
