using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Validators
{
    public class NotFutureIntYear : ValidationAttribute
    {
        public string? Name { get; set; }

        public override bool IsValid(object? value)
        {
            if (value is null)
            {
                return true;
            }

            if (value is not int year)
            {
                return false;
            }

            return year <= DateTime.UtcNow.Year;
        }


        public override string FormatErrorMessage(string name)
        {
            return $"{Name ?? name} musi być obecynm rokiem lub w przeszłości!";
        }
    }
}
