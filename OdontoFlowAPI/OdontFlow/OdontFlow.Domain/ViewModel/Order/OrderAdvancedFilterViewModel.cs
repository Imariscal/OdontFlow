namespace OdontFlow.Domain.ViewModel.Order;

public class OrderAdvancedFilterViewModel
{
    public string? Search { get; set; }
    public ORDER_STATUS? OrderStatusId { get; set; }
    public ORDER_TYPE? OrderTypeId { get; set; }
    public int? GroupId { get; set; }
    public string? ClientName { get; set; }
    public string? PatientName { get; set; }
    public string? RequesterName { get; set; }
    public DateTime? CreationDateStart { get; set; }
    public DateTime? CreationDateEnd { get; set; }
    public DateTime? CommitmentDateStart { get; set; }
    public DateTime? CommitmentDateEnd { get; set; }
    public bool? PaymentComplete { get; set; }
    public bool? ApplyInvoice { get; set; }
    public decimal? MinBalance { get; set; }
    public decimal? MaxBalance { get; set; }
    public string? Barcode { get; set; }
    public PAYMENT_TYPE? PaymentTypeId { get; set; }
    public List<Guid>? ProductIds { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
