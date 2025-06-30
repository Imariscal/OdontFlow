using Microsoft.EntityFrameworkCore;
using OdontFlow.Domain.Entities;
using OdontFlow.Persistence.Contexts.Base;
using System.Linq.Expressions;

namespace OdontFlow.Persistence.Contexts;

public class ReadOnlyContext : DbContext, IReadOnlyContext
{
    public bool IsReadOnly => true;
    public bool IsWriteOnly => false;

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<PriceList> PriceLists { get; set; }
    public DbSet<PriceListItem> PriceListItems { get; set; } = default!;
    public DbSet<Client> Clients { get; set; }
    public DbSet<ClientInvoice> ClientInvoice { get; set; }
    public DbSet<WorkStation> WorkStations { get; set; }
    public DbSet<WorkPlan> WorkPlans { get; set; }
    public DbSet<WorkPlanProducts> WorkPlanProducts { get; set; }
    public DbSet<WorkStationPlan> WorkStationPlan { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderPayment> OrderPayments { get; set; }
    public DbSet<WorkingHour> WorkingHours { get; set; }
    public DbSet<Holiday> Holidays { get; set; }
    public DbSet<StationWork> StationWorks { get; set; }

    public DbSet<OrderSequence> OrderSequences { get; set; }
    public ReadOnlyContext() { }
    public ReadOnlyContext(DbContextOptions<ReadOnlyContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadOnlyContext).Assembly);

        modelBuilder.Entity<PriceListItem>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<PriceListItem>()
            .HasOne(x => x.Product)
            .WithMany(p => p.PriceListItems)
            .HasForeignKey(x => x.ProductId);

        modelBuilder.Entity<PriceListItem>()
            .HasOne(x => x.PriceList)
            .WithMany(p => p.Items)
            .HasForeignKey(x => x.PriceListId);

        modelBuilder.Entity<PriceListItem>()
            .HasIndex(x => new { x.ProductId, x.PriceListId })
            .IsUnique();

        // 🔧 Agrega esto para evitar múltiples cascade paths
        modelBuilder.Entity<Client>()
            .HasOne(c => c.Employee)
            .WithMany() // no necesitas .WithMany(e => e.Clients)
            .HasForeignKey(c => c.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Client>()
            .Property(c => c.Credit)
            .HasPrecision(18, 2);  
        
       //  SetFilter(modelBuilder);
    }

    private void SetFilter(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var isActiveProperty = entityType.FindProperty("Deleted");
            if (isActiveProperty != null && isActiveProperty.ClrType == typeof(bool))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "p");
                var filter = Expression.Lambda(Expression.Property(parameter, isActiveProperty.PropertyInfo!), parameter);
                entityType.SetQueryFilter(filter);
            }
        }
    }
    public override int SaveChanges() =>
        throw new InvalidOperationException("This context is a read-only one.");

    public override int SaveChanges(bool acceptAllChangesOnSuccess) =>
        throw new InvalidOperationException("This context is a read-only one.");

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("This context is a read-only one.");

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("This context is a read-only one.");
}
