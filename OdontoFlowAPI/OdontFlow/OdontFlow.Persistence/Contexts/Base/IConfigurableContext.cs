namespace OdontFlow.Persistence.Contexts.Base;

public interface IConfigurableContext
{
    bool IsReadOnly { get; }
    bool IsWriteOnly { get; }
}