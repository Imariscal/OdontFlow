namespace OdontFlow.Domain.DTOs.Product;

public class UpdateProductDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public PRODUCT_CATEGORY ProductCategory { get; set; }
    public float Price { get; set; }
    public bool IsLabelRequired { get; set; }
    public bool ApplyDiscount { get; set; }

    public bool Active { get; set; }
}
