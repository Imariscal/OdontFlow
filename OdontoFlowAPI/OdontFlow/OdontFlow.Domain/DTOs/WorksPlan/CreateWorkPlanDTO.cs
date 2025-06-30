namespace OdontFlow.Domain.DTOs.WorksPlan;

public class CreateWorkPlanDTO
{
    public string Name { get; set; } = default!;

    public List<CreateWorkStationPlanDTO> Stations { get; set; } = new();
    public List<Guid> ProductIds { get; set; } = new(); // Solo se necesita el ID
}
