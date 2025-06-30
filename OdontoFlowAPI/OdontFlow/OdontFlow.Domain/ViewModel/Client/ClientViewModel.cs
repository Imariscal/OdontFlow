namespace OdontFlow.Domain.ViewModel.Client;

public class ClientViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Contact { get; set; }
    public string Phone1 { get; set; }
    public string? Phone2 { get; set; }
    public string? Mobile { get; set; }
    public string GeneralEmail { get; set; }
    public string? CollectionEmail { get; set; }
    public decimal Credit { get; set; }
    public bool AppliesInvoice { get; set; }
    public int StateId { get; set; }
    public string? Remarks { get; set; }
    public string? SalesEmployee { get; set; }
    public ClientInvoiceViewModel? ClientInvoice { get; set; }
    public Guid? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public ClientViewModel? Client { get; set; }
    public int GroupId { get; set; }
    public string? WorkGroup { get; set; }
    public Guid? PriceListId { get; set; }
    public string? PriceList { get; set; }
    public decimal CommissionPercentage { get; set; }
    public bool Active { get; set; }

}