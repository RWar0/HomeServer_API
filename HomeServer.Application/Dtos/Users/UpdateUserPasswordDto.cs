using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.Users
{
    public class UpdateUserPasswordDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Hasło musi zostać podane!")]
        public string Password { get; set; } = default!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Potwierdzenie hasła musi zostać podane!")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
