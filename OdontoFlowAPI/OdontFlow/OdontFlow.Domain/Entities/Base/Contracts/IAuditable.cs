namespace OdontFlow.Domain.Entities.Base.Contracts;

public interface IAuditable<T> : IBaseEntity<T>
{
    string CreatedBy { get; set; }
    DateTime CreationDate { get; set; }
    string LastModifiedBy { get; set; }
    DateTime LastModificationDate { get; set; }
    string? DeletedBy { get; set; }
    DateTime? DeletionDate { get; set; }
    bool Active { get; set; }
    bool Deleted { get; set; }
}
