
namespace OdontFlow.Domain.DTOs.PriceList;

public class CreatePriceListDTO
{ 
    public string Name { get; set; } = default!;
    public LIST_CATEGORY Category { get; set; } = default!;
    public float Discount { get; set; }
    public bool Active { get; set; }
}
