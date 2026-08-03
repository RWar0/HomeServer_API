using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Validators
{
    class AllowedContentTypesAttribute : ValidationAttribute
    {
        private readonly string[] _contentTypes;
        public AllowedContentTypesAttribute(string[] contentTypes) => _contentTypes = contentTypes;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var contentType = file.ContentType;
                if (!_contentTypes.Contains(contentType.ToLower(), StringComparer.OrdinalIgnoreCase))
                    return new ValidationResult(ErrorMessage ?? "Niedozwolony format pliku.");
            }

            return ValidationResult.Success;
        }
    }
}
