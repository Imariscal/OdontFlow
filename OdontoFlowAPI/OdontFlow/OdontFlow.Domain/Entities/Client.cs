using OdontFlow.Domain.Entities.Base;
namespace OdontFlow.Domain.Entities;
public class Client : Auditable<Guid>
{    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Contact { get; set; } = default!;
    public string Phone1 { get; set; } = default!;
    public string? Phone2 { get; set; }
    public string? Mobile { get; set; }
    public string GeneralEmail { get; set; } = default!;
    public string? CollectionEmail { get; set; }
    public decimal Credit { get; set; }
    public bool AppliesInvoice { get; set; }
    public int GroupId { get; set; }
    public int StateId { get; set; }
    public string? Remarks { get; set; }
    public Guid PriceListId { get; set; } 
    public string? Account { get; set; } 
    public ClientInvoice? ClientInvoice { get; set; }
    public PriceList? PriceList { get; set; }
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;
    public decimal CommissionPercentage { get; set; } 
}
