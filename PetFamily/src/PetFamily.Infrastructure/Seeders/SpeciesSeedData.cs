using PetFamily.Domain.PetManagment.Entities;

namespace PetFamily.Infrastructure.Seeders;

public class SpeciesSeedData
{
    public static readonly Species Dog = new()
    {
        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        Title = "Собака"
    };

    public static readonly Species Cat = new()
    {
        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
        Title = "Кошка"
    };

    public static readonly Species Bird = new()
    {
        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
        Title = "Птица"
    };

    public static readonly Species Rodent = new()
    {
        Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
        Title = "Грызун"
    };

    public static List<Species> GetAll() => [Dog, Cat, Bird, Rodent];
}
