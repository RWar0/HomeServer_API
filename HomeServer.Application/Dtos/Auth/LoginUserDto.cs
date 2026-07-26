using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.Auth
{
    public class LoginUserDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Nie podano login'u!")]
        public string Login { get; set; } = default!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Nie podano hasła!")]
        public string Password { get; set; } = default!;
    }
}
