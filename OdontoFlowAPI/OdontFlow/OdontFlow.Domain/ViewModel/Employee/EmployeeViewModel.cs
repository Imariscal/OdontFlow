namespace OdontFlow.Domain.ViewModel.Employee;

public class EmployeeViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool ApplyCommission { get; set; }
    public bool IsSalesRepresentative { get; set; }
    public float CommissionPercentage { get; set; }
    public bool Active { get; set; }
    public List<EmployeeClientSummaryViewModel>? Clients { get; set; }
}

 