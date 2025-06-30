
using OdontFlow.CrossCutting.Validators.Contracts;

namespace OdontFlow.CrossCutting.Exceptions;


public class BusinessValidationException : ApplicationException
{
    public Dictionary<string, string[]> Errors { get; } = [];

    public BusinessValidationException() :
        base("There was on or more errors during the valitadion.")
    { Errors = []; }

    public BusinessValidationException(IEnumerable<IValidationFailure> failures) : this()
    {
        Errors = failures.GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(fg => fg.Key, fg => fg.ToArray());

    }
}