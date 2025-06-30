using OdontFlow.Domain.Entities.Base;

namespace OdontFlow.Domain.Entities;

public class WorkStationPlan : Auditable<Guid>
{ 
    public Guid WorkStationId { get; set; }
    public Guid PlanId { get; set; }

    public WorkPlan Plan { get; set; } = default!;
    public WorkStation WorkStation { get; set; } = default!;
    public int Order { get; set; } = 1;
}
