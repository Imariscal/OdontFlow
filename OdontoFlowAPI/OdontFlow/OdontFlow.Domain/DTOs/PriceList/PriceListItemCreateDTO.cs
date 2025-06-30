namespace OdontFlow.Domain.DTOs.PriceList;

public class PriceListItemCreateDTO
{
    public Guid ProductId { get; set; }
    public Guid PriceListId { get; set; }
    public decimal Price { get; set; }
    public string? Comments { get; set; }
}
