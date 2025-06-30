using OdontFlow.Domain.Entities.Base.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace OdontFlow.Persistence.Configuration.Base;

public class BaseEntityConfiguration<TEntity, TKey> :
    IEntityTypeConfiguration<TEntity>
    where TEntity : class, IAuditable<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(p => p.CreatedBy).HasMaxLength(50).IsRequired();
        builder.Property(p => p.CreationDate).HasColumnType("datetime2").IsRequired().HasDefaultValueSql("GETUTCDATE()");
        builder.Property(p => p.LastModifiedBy).HasMaxLength(50).IsRequired(false);
        builder.Property(p => p.LastModificationDate).HasColumnType("datetime2").IsRequired().HasDefaultValueSql("GETUTCDATE()");
        builder.Property(p => p.DeletedBy).HasMaxLength(50).IsRequired(false);
        builder.Property(p => p.DeletionDate).HasColumnType("smalldatetime");
        builder.Property(p => p.Active).IsRequired().HasDefaultValue(true);
    }
}
