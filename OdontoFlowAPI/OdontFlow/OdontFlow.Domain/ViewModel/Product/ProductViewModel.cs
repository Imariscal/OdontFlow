namespace OdontFlow.Domain.ViewModel.Product;

public class ProductViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string ProductCategory { get; set; } = default!;
    public string PriceFormatted { get; set; } = default!;
    public decimal Price  { get; set; } = default!;
    public bool IsLabelRequired { get; set; }
    public bool ApplyDiscount { get; set; }
    public int ProductCategoryId { get; set; } = default!;
    public bool Active { get; set; }
}
