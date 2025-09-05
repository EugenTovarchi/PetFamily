using PetFamily.Domain.PetManagment.ValueObjects;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Shared.Extensions;

public static class EnumExtension
{
    public static Result<PetStatus> ParsePetStatus(string status)
    {
        return status.ToLower() switch
        {
            "lookingtreatment" => PetStatus.LookingTreatment,
            "lookinghome" => PetStatus.LookingHome,
            "hashome" => PetStatus.HasHome,
            _ => Errors.General.ValueIsInvalid(nameof(status))
        };
    }

    public static Result<PetColor> ParsePetColor(string color)
    {
        var colorLower = color.ToLower().Trim();
        return colorLower switch
        {
            "чёрный" or "черный" => PetColor.Black,
            "белый" => PetColor.White,
            "коричневый" => PetColor.Brown,
            "серый" => PetColor.Gray,
            "рыжий" => PetColor.Ginger,
            "кремовый" => PetColor.Cream,
            "золотистый" => PetColor.Golden,
            "пятнистый" => PetColor.Spotted,
            "полосатый" => PetColor.Striped,
            "смешанный" => PetColor.Mixed,
            "чёрно-белый" or "черно-белый" => PetColor.BlackAndWhite,

            _ => Errors.General.ValueIsInvalid(nameof(color))
        };
    }   
}
