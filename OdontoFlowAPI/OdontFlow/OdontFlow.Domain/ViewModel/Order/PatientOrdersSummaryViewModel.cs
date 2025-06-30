using OdontFlow.Domain.ViewModel.OrderPayment;

namespace OdontFlow.Domain.ViewModel.Order;

public class ClientOrdersSummaryViewModel
{
    public string ClientName { get; set; } = string.Empty;
    public List<PatientOrdersSummaryViewModel> Patients { get; set; } = new();
}
public class PatientOrdersSummaryViewModel
{ 
    public string PatientName { get; set; } = string.Empty;
    public int PaidCount { get; set; }
    public int DebtCount { get; set; }
    public List<OrderPaymentViewModel> Payments { get; set; } = new();
    public List<OrderViewModel> DebtOrders { get; set; } = new();
}
