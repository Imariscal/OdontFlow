namespace OdontFlow.Domain.DTOs.WorksPlan;

public class UpdateWorkPlanDTO
{

    public Guid Id { get; set; }
    public string Name { get; set; } = default!;

    public List<CreateWorkStationPlanDTO> Stations { get; set; } = new();
    public List<Guid> ProductIds { get; set; } = new();

    public bool Active { get; set; }
}
