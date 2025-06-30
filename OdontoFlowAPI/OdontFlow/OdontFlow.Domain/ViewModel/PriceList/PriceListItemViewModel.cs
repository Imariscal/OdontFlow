using OdontFlow.Domain.ViewModel.Product;

namespace OdontFlow.Domain.ViewModel.PriceList;

public class PriceListItemViewModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; } 
    public Guid PriceListId { get; set; }
    public string PriceListName { get; set; } = default!;
    public decimal Price { get; set; }
    public ProductViewModel Product { get; set; } = default!;
    public string? Comments { get; set; }
}