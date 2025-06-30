
using OdontFlow.Domain.Entities.Base;

namespace OdontFlow.Domain.Entities;

public class OrderPayment : Auditable<Guid>
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;
    public PAYMENT_TYPE PaymentTypeId { get; set; } 
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
}
