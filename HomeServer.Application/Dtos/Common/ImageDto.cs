using HomeServer.Application.Validators;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace HomeServer.Application.Dtos.Common
{
    public class ImageDto
    {
        [Required(ErrorMessage = "Nie przesłano pliku!")]
        [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "Maksymalny rozmiar pliku to 10 MB!")]
        [AllowedExtensions([".jpg", ".jpeg", ".png"], ErrorMessage = "Plik musi być zdjęciem!")]
        [AllowedContentTypes(["image/jpg", "image/jpeg", "image/png"], ErrorMessage = "Plik musi być przesłane jako zdjęcie!")]
        public IFormFile File { get; set; } = default!;
    }
}
