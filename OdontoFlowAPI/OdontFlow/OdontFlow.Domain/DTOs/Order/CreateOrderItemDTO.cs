namespace OdontFlow.Domain.DTOs.Order;

public class CreateOrderItemDTO
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }

    public List<PType> Teeth { get; set; } = new();
}
