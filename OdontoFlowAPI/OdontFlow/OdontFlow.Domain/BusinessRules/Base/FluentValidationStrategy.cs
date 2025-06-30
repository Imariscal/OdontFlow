using OdontFlow.CrossCutting.Validators.Contracts;
using OdontFlow.Domain.BusinessRules.Base;
using FluentValidation; 

namespace OdontFlow.Retail.Domain.BusinessRules.Base;

public class FluentValidationStrategy<T>(IValidator<T> validator) : IValidationStrategy<T>
{
    private readonly IValidator<T> _validator = validator;

    public ValidationResult Validate(T instance)
    {
        var result = _validator.Validate(instance);
        var validationResult = new ValidationResult
        {
            IsValid = result.IsValid,
            Errors = FluentValidationStrategy<T>.GetErrors(result.Errors)
        };

        return validationResult;
    }

    private static List<IValidationFailure> GetErrors(List<FluentValidation.Results.ValidationFailure> errors)
    {
        var validationFailures = new List<IValidationFailure>();

        foreach (var error in errors)
        {
            validationFailures.Add(new ValidationFailure(error.PropertyName, error.ErrorMessage));
        }

        return validationFailures;
    }
}