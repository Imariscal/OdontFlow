namespace OdontFlow.CrossCutting.Validators.Contracts;

public interface IValidationFailure
{
    string PropertyName { get; set; }
    string ErrorMessage { get; set; }
}
