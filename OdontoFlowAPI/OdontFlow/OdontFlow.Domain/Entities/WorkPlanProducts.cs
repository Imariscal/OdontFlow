using OdontFlow.Domain.Entities.Base;

namespace OdontFlow.Domain.Entities;

public class WorkPlanProducts : Auditable<Guid>
{
    public Guid ProductId { get; set; }
    public Guid PlanId { get; set; }
    public WorkPlan Plan { get; set; } = default!;
    public Product Product { get; set; } = default!;
}
