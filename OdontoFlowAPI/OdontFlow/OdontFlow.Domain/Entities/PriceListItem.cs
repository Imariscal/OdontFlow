using OdontFlow.Domain.Entities.Base;

namespace OdontFlow.Domain.Entities;

public class PriceListItem : Auditable<Guid>
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;
    public Guid PriceListId { get; set; }
    public PriceList PriceList { get; set; } = default!;
    public decimal Price { get; set; }
    public string? Comments { get; set; }
}