namespace OdontFlow.Domain.DTOs.PriceList;
public class PriceListItemUpdateDTO
{
    public Guid Id { get; set; } 
    public decimal Price { get; set; }

    public string? Comments { get; set; }
}
