using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.Auth
{
    public class LogoutDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Brak tokenu odświeżającego!")]
        public string RefreshToken { get; set; } = default!;
    }
}
