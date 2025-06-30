namespace OdontFlow.Domain.DTOs.Order;

public class CreateOrderDTO
{
    public Guid ClientId { get; set; }
    public string PatientName { get; set; } = default!;
    public string RequesterName { get; set; } = default!;
    public int OrderTypeId { get; set; }
    public DateTime CommitmentDate { get; set; }
    public string? Color { get; set; }
    public bool ApplyInvoice { get; set; }
    public string? Others { get; set; }
    public bool Bite { get; set; }
    public bool Models { get; set; }
    public bool Casts { get; set; }
    public bool Spoons { get; set; }
    public bool Attachments { get; set; }
    public bool Analogs { get; set; }
    public bool Screws { get; set; }
    public bool MetalArticulator { get; set; }
    public bool DisposableArticulator { get; set; }
    public List<CreateOrderItemDTO> Items { get; set; } = new();
    public string? Observations { get; set; }
    public string? CollectionNotes { get; set; }
    public string? DeliveryNotes { get; set; }

    public bool ? Uncollectible { get; set; }
}
