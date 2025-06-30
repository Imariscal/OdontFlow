
namespace OdontFlow.Domain.DTOs.OrderPayment;

public class UpdateOrderPaymentDTO
{
    public Guid Id { get; set; }
    public PAYMENT_TYPE PaymentTypeId { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
}
