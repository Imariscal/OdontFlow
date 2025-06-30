namespace OdontFlow.Domain.ViewModel.WorkPlan;

public class WorkStationPlanViewModel
{
    public Guid WorkStationId { get; set; }
    public string WorkStationName { get; set; } = default!;
    public int Order { get; set; }
}
