namespace OdontFlow.Domain.ViewModel.Employee;
public class EmployeeClientSummaryViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public float CommissionPercentage { get; set; }
}
