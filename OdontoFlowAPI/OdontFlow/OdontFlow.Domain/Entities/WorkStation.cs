using OdontFlow.Domain.Entities.Base;
namespace OdontFlow.Domain.Entities;

public class WorkStation : Auditable<Guid>
{
    public string Name { get; set; } = default!;

    public int Days { get; set; } = 1;
}
