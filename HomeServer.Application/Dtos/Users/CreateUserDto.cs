using HomeServer.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.Users
{
    public class CreateUserDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Imię i nazwisko musi zostać podane!")]
        public string Name { get; set; } = default!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Nazwa użytkownika musi zostać podana!")]
        public string Username { get; set; } = default!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email musi zostać podany!")]
        [EmailAddress(ErrorMessage = "Email posiada niepoprawny format!")]
        [RegularExpression(@"^[^@\s]{2,}@[a-zA-Z0-9-]{2,}\.[a-zA-Z]{2,}$", ErrorMessage = "Email posiada niepoprawny format!")]
        public string Email { get; set; } = default!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Hasło musi zostać podane!")]
        public string Password { get; set; } = default!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Potwierdzenie hasła musi zostać podane!")]
        public string ConfirmPassword { get; set; } = default!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Rola musi zostać wybrana!")]
        public RolesEnum Role { get; set; }
    }
}
