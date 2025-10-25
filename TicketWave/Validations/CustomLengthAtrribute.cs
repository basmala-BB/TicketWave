using System.ComponentModel.DataAnnotations;
namespace TicketWave.Validations
{
    public class CustomLengthAtrribute : ValidationAttribute
    {
        private readonly int minlength;
        private readonly int maxlength;

        public CustomLengthAtrribute(int minlength , int maxlength )
        {
            this.minlength = minlength;
            this.maxlength = maxlength;
        }
        public override bool IsValid(object? value)
        {
            if ( value is string result)
            {
                if (result.Length >= minlength && result.Length <= maxlength)
                    return true;
            }
            return false;
        }
        public override string FormatErrorMessage(string name)
        {
            return $"the filed {name} must be between {minlength} and {maxlength}";
        }
    }
}
