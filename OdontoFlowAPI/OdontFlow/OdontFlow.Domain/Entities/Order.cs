using OdontFlow.Domain.Entities.Base;

namespace OdontFlow.Domain.Entities;

public class Order : Auditable<Guid>
{
    public ORDER_STATUS OrderStatusId { get; set; }
    public ORDER_TYPE OrderTypeId { get; set; }
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = default!;
    public string? RequesterName { get; set; }
    public string PatientName { get; set; } = default!;
    public bool Bite { get; set; }
    public bool Models { get; set; }
    public bool Casts { get; set; }
    public bool Spoons { get; set; }
    public bool Attachments { get; set; }
    public bool Analogs { get; set; }
    public bool Screws { get; set; }
    public string? Others { get; set; }
    public string? Observations { get; set; }       
    public DateTime? DeliveryDate { get; set; }
    public DateTime ConfirmDate { get; set; }
    public DateTime CommitmentDate { get; set; }
    public DateTime? ProcessDate { get; set; }
    public DateTime? CompleteDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public bool PaymentComplete { get; set; }
    public bool MetalArticulator { get; set; }
    public bool DisposableArticulator { get; set; }
    public decimal Subtotal { get; set; }  
    public decimal Tax { get; set; }
    public decimal Total { get; set; } 
    public decimal Payment { get; set; }
    public decimal Balance { get; set; }
    public bool ApplyInvoice { get; set; }
    public string? CollectionNotes { get; set; }
    public string? DeliveryNotes { get; set; }
    public Guid? PreviousOrderId { get; set; }
    public Order? PreviousOrder { get; set; }
    public string? Color { get; set; } 
    public bool Uncollectible { get; set; }
    public string? InvoiceNumber { get; set; } 
    public string Barcode { get; set; } = default!;
    public decimal CommissionPercentage { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<OrderPayment> Payments { get; set; } = new List<OrderPayment>();
    public ICollection<StationWork> StationWorks { get; set; } = new List<StationWork>();

}
