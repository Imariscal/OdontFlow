using OdontFlow.Domain.ViewModel.Client;
using OdontFlow.Domain.ViewModel.OrderPayment;
using OdontFlow.Domain.ViewModel.StationWork;

namespace OdontFlow.Domain.ViewModel.Order;

public class OrderViewModel
{
    public Guid Id { get; set; }
    public string WorkGroup { get; set; } = default!;
    public int OrderStatusId { get; set; }
    public string OrderStatus { get; set; } = default!;
    public int OrderTypeId { get; set; }
    public string OrderType { get; set; } = default!;
    public Guid ClientId { get; set; }
    public string? ClientName { get; set; }
    public string RequesterName { get; set; } = default!;
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
    public DateTime CreationDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public DateTime CommitmentDate { get; set; }
    public DateTime ConfirmDate { get; set; }
    public DateTime ProcessDate { get; set; }
    public DateTime? PaymentDate { get; set; } // 👈 Ahora nullable
    public bool PaymentComplete { get; set; }
    public bool MetalArticulator { get; set; }
    public bool DisposableArticulator { get; set; }

    public decimal Subtotal { get; set; }    // 👈 Agregado
    public decimal Tax { get; set; }
    public decimal Total { get; set; }        // 👈 Agregado
    public decimal Payment { get; set; }
    public decimal Balance { get; set; }

    public string? CollectionNotes { get; set; }
    public string? DeliveryNotes { get; set; }
    public Guid? PreviousOrderId { get; set; }
    public string? Color { get; set; }
    public bool Uncollectible { get; set; }
    public bool ApplyInvoice { get; set; }
    public string? InvoiceNumber { get; set; }

    public List<OrderItemViewModel> Items { get; set; } = new();
    public List<OrderPaymentViewModel> Payments { get; set; } = new();
    public ClientViewModel? Client { get; set; }
    public string Barcode { get; set; } = default!;
    public StationWorkCurrentViewModel CurrentStationWork { get; set; }

    public int? DaysInDebt { get; set; }
}
