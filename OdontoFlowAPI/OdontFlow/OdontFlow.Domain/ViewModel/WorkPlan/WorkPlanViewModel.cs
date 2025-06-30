using OdontFlow.Domain.ViewModel.WorkStation;

namespace OdontFlow.Domain.ViewModel.WorkPlan;
public class WorkPlanViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;

    public List<WorkStationPlanViewModel> Stations { get; set; } = new();
    public List<WorkPlanProductViewModel> Products { get; set; } = new();

    public bool Active { get; set; }
}
