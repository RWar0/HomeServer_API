using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Validators
{
    public class NotFutureDateAttribute : ValidationAttribute
    {
        public string? Name { get; set; }

        public override bool IsValid(object? value)
        {
            if (value is null)
            {
                return true;
            }

            if (value is not DateOnly date)
            {
                return false;
            }

            return date <= DateOnly.FromDateTime(DateTime.Today);
        }


        public override string FormatErrorMessage(string name)
        {
            return $"{Name ?? name} musi być datą w przeszłości!";
        }
    }
}
