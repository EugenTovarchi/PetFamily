using System.ComponentModel.DataAnnotations;

namespace PetFamily.Domain.Pets;

public enum PetColor
{
    [Display(Name = "Чёрный")] Black,
    [Display(Name = "Белый")] White,
    [Display(Name = "Коричневый")] Brown,
    [Display(Name = "Серый")] Gray,
    [Display(Name = "Рыжий")] Ginger,
    [Display(Name = "Кремовый")] Cream,
    [Display(Name = "Золотистый")] Golden,
    [Display(Name = "Пятнистый")] Spotted,
    [Display(Name = "Полосатый")] Striped,
    [Display(Name = "Смешанный")] Mixed,
    [Display(Name = "Чёрно-белый")] BlackAndWhite
}


