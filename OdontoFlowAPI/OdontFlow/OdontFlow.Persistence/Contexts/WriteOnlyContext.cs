using Microsoft.EntityFrameworkCore;
using OdontFlow.Domain.Entities;
using OdontFlow.Persistence.Contexts.Base;

namespace OdontFlow.Persistence.Contexts;

public class WriteOnlyContext(DbContextOptions<WriteOnlyContext> options)
    : DbContext(options), IWriteOnlyContext
{
    public bool IsReadOnly => false;
    public bool IsWriteOnly => true;
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<PriceList> PriceLists { get; set; }
    public DbSet<PriceListItem> PriceListItems { get; set; } = default!;
    public DbSet<Client> Clients { get; set; }
    public DbSet<WorkStation> WorkStations { get; set; }
    public DbSet<ClientInvoice> ClientInvoice { get; set; }
    public DbSet<WorkPlan> WorkPlans { get; set; }
    public DbSet<WorkPlanProducts> WorkPlanProducts { get; set; }
    public DbSet<WorkStationPlan> WorkStationPlan { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<OrderPayment> OrderPayments { get; set; }
    public DbSet<WorkingHour> WorkingHours { get; set; }
    public DbSet<Holiday> Holidays { get; set; }
    public DbSet<StationWork> StationWorks { get; set; }
    public DbSet<OrderSequence> OrderSequences { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteOnlyContext).Assembly);
    }
}
