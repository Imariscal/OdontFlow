using OdontFlow.Domain.Entities.Base;

namespace OdontFlow.Domain.Entities;

public class OrderItem : Auditable<Guid>
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;
    public int Quantity { get; set; }

    public decimal UnitTax { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public List<PType> Teeth { get; set; } = new();
}
