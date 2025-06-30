using OdontFlow.Domain.Entities.Base;
namespace OdontFlow.Domain.Entities;
public class Product : Auditable<Guid>
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public bool IsLabelRequired { get; set; } = false;
    public bool ApplyDiscount { get; set; } = false;
    public PRODUCT_CATEGORY ProductCategory { get; set; }
    public ICollection<PriceListItem> PriceListItems { get; set; } = new List<PriceListItem>();

    public ICollection<WorkPlanProducts> WorkPlans { get; set; } = new List<WorkPlanProducts>();

}
