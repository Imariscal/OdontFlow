using OdontFlow.Domain.DTOs.Contracts;
namespace OdontFlow.Domain.DTOs.Base;

public abstract class BaseCrudDTO<T> : IBaseCrudDTO<T>
{
    public required T Id { get; set; }
}