using OdontFlow.Domain.Entities.Base;

namespace OdontFlow.Domain.Entities;

public class PriceList : Auditable<Guid>
{
    public string Name { get; set; } = default!;
    public PRODUCT_CATEGORY Category { get; set; }
    public float Discount { get; set; } = 0f;

    public ICollection<PriceListItem> Items { get; set; } = new List<PriceListItem>();
}
