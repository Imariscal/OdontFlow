namespace OdontFlow.Domain.ViewModel.Report;

public class DebtSummaryViewModel
{
    public int TotalOrders { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal MaxSingleDebt { get; set; }
    public int MaxDaysInDebt { get; set; }
    public int AvgDaysInDebt { get; set; }
}
