namespace OdontFlow.CrossCutting.Exceptions;
public class NotFoundException : ApplicationException
{
    public NotFoundException() { }
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string name, object key) :
        base($"The Record \"{name}\" ({key}) does not exists.")
    { }
}
