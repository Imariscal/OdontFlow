namespace OdontFlow.Domain.ViewModel.OrderPayment;

public class OrderPaymentViewModel
{
    public Guid Id { get; set; }
    public string Barcode { get; set; }
    public string PatientName { get; set; }
    public string ClientName { get; set; }
    public PAYMENT_TYPE PaymentTypeId { get; set; }
    public string PaymentType { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
    public DateTime CreationDate { get; set; }
}
