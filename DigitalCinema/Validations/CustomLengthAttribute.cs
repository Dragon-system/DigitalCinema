using System.ComponentModel.DataAnnotations;

namespace DigitalCinema.Validations
{
    public class CustomLengthAttribute : ValidationAttribute
    {

        private readonly int _MinLength;
        private readonly int _MaxLength;
        public CustomLengthAttribute(int minLength, int maxLength)
        {
            _MinLength = minLength;
            _MaxLength = maxLength;
        }
        public override bool IsValid(object? value)
        {
            if (value == null) return false;

            if (value is string str)
            {
                if (str.Length >= _MinLength && str.Length < _MaxLength)
                {
                    return true;
                }
            }
            return false;
        }

      public override string FormatErrorMessage(string name)
        {
            return $"{name} must be between {_MinLength} and {_MaxLength} characters.";
        }
    }
}
