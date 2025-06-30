using OdontFlow.Domain.Entities.Base;
namespace OdontFlow.Domain.Entities;

public class WorkPlan : Auditable<Guid>
{
    public string Name { get; set; } = default!;
    public ICollection<WorkStationPlan> Stations { get; set; } = new List<WorkStationPlan>();
    public ICollection<WorkPlanProducts> Products { get; set; } = new List<WorkPlanProducts>();

}
