using OdontFlow.Domain.Entities.Base;

namespace OdontFlow.Domain.Entities;

public class ClientInvoice : Auditable<Guid>
{
    public Guid? ClientId { get; set; }
    public Client? Client { get; set; }
    public string? InvoiceName { get; set; }
    public string? InvoiceRFC { get; set; }
    public string? InvoiceEmail { get; set; }
    public string? Phone { get; set; }
    public string? CFDIUse { get; set; } 
    public string? Regimen { get; set; } 
    public string? Street { get; set; }
    public string? ExteriorNumber { get; set; }
    public string? InteriorNumber { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }
    public string? Municipality { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }

}
