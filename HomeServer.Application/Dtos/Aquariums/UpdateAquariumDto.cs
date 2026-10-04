using HomeServer.Application.Validators;
using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.Aquariums
{
    public class UpdateAquariumDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Nazwa jest wymagana!")]
        public string Name { get; set; } = default!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Litraż jest wymagany!")]
        [Range(1, 100000, ErrorMessage = "Litraż musi być dodatni, nie większy niż 10000!")]
        public int Volume { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Data utworzenia jest wymagana!")]
        [NotFutureDate(Name = "Data założenia akwarium")]
        public DateOnly CreationDate { get; set; }
    }
}
