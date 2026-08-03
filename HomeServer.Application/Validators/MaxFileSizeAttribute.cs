using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Validators
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize) => _maxFileSize = maxFileSize;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (file.Length > _maxFileSize)
                    return new ValidationResult(ErrorMessage ?? $"Maksymalny rozmiar pliku to {_maxFileSize} bajtów.");
            }
            return ValidationResult.Success;
        }
    }
}
