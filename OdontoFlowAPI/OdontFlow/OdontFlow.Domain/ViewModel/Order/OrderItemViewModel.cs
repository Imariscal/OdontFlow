namespace OdontFlow.Domain.ViewModel.Order;

public class OrderItemViewModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }

    public int Quantity { get; set; }

    public decimal UnitCost { get; set; }
    public decimal UnitTax { get; set; }          // 👈 Agregado: IVA unitario
    public decimal TotalCost { get; set; }    // 👈 Total ya con IVA si aplica

    public List<PType> Teeth { get; set; } = new(); // 👈 Siempre inicializado para evitar null

    public string TeethNames => GetTeethNames();

    private string GetTeethNames()
    {
        if (Teeth == null || !Teeth.Any())
            return string.Empty;

        return string.Join(", ", Teeth.Select(GetToothCode));
    }

    private string GetToothCode(PType tooth)
    {
        return tooth switch
        {
            PType.SUPERIOR => "SUPERIOR",
            PType.INFERIOR => "INFERIOR",
            _ => $"P{(int)tooth}"
        };
    }

}

