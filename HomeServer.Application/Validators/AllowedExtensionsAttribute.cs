using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Validators
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions) => _extensions = extensions;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower(), StringComparer.OrdinalIgnoreCase))
                    return new ValidationResult(ErrorMessage ?? "Niedozwolony format pliku.");
            }
            return ValidationResult.Success;
        }
    }
}
