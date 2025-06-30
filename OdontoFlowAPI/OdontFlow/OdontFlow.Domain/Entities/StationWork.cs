using OdontFlow.Domain.Entities.Base;
namespace OdontFlow.Domain.Entities;

public class StationWork : Auditable<Guid>
{
    public Guid WorkStationId { get; set; }
    public WorkStation WorkStation { get; set; }
    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public Guid? ProductId { get; set; }
    public Product? Product { get; set; }
    public Guid? OrderId { get; set; }
    public Order? Order { get; set; }
    public WORK_STATUS WorkStatus { get; set; } 
    public DateTime StationStartDate { get; set; }
    public DateTime StationEndDate { get; set; }
    public DateTime EmployeeStartDate { get; set; }
    public DateTime EmployeeEndDate { get; set; }
    public string Barcode { get; set; }
    public bool InProgress { get; set; }
    public int Step { get; set; }
    public bool OrderCanceled {  get; set; } = false;
    public DateTime? BlockedDate { get; set; }
    public DateTime? UnblockedDate { get; set; }
}
