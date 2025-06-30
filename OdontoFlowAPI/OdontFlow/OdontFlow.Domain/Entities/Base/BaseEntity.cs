
using OdontFlow.Domain.Entities.Base.Contracts;
using System.ComponentModel.DataAnnotations;

namespace OdontFlow.Domain.Entities.Base;

public abstract class BaseEntity<T> : IBaseEntity<T>
{
    [Key]
    public required T Id { get; set; }
}
