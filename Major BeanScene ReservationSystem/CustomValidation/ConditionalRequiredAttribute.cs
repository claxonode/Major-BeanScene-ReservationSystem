using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.CustomValidation
{
    public class ConditionalRequiredAttribute : ValidationAttribute
    {
        private readonly string dependentProperty;

        public ConditionalRequiredAttribute(string dependentProperty)
        {
            this.dependentProperty = dependentProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var dependentValue = instance.GetType().GetProperty(dependentProperty).GetValue(instance, null);

            if (dependentValue is bool isForSomeoneElse && isForSomeoneElse && value == null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
