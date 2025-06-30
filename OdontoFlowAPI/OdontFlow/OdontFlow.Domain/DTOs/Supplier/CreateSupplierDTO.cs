namespace OdontFlow.Domain.DTOs.Supplier;
public class CreateSupplierDTO
{
    public string Name { get; set; }
    public string Contact { get; set; }
    public string Email { get; set; }
    public string Phone1 { get; set; }
    public string Phone2 { get; set; }
    public string BankDetails { get; set; }
    public string InvoiceContact { get; set; }
    public string InvoiceEmail { get; set; }
    public string InvoicePhone { get; set; }
    public string Account { get; set; }
    public decimal? Credit { get; set; }
    public string CreditFormatted { get; set; }
    public string Comments { get; set; }
    public string Product { get; set; }
    public string Address { get; set; }
    public bool Active { get; set; }
}
