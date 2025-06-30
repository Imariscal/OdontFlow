
using OdontFlow.Domain.Entities;

namespace OdontFlow.Domain.ViewModel.PriceList;

public class PriceListViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Category { get; set; } = default!; 
    public float Discount { get; set; }
    public int CategoryId { get; set; } = default!;
    public ICollection<PriceListItemViewModel> Items { get; set; } = new List<PriceListItemViewModel>();
    public bool Active { get; set; } = default!;
}
