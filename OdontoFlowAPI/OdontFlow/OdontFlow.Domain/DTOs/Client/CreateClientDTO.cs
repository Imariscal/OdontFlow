using OdontFlow.Domain.DTOs.Employee;

namespace OdontFlow.Domain.DTOs.Client;

public class CreateClientDTO
{
    public bool Active { get; set; }
    public string? Account { get; set; }
    public bool AppliesInvoice { get; set; }
    public string Address { get; set; }
    public string? CollectionEmail { get; set; }
    public ClientInvoiceDTO? ClientInvoice { get; set; }
    public string Contact { get; set; }
    public decimal Credit { get; set; }
    public int GroupId { get; set; }
    public string GeneralEmail { get; set; }
    public string Name { get; set; }
    public string? Mobile { get; set; }
    public string? Phone2 { get; set; }
    public string Phone1 { get; set; }
    public Guid PriceListId { get; set; }
    public string? Remarks { get; set; }
    public Guid? EmployeeId { get; set; }
    public int StateId { get; set; }
    public float CommissionPercentage { get; set; }

}
