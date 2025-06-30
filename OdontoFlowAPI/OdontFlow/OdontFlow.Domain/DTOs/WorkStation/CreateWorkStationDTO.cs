namespace OdontFlow.Domain.DTOs.WorkStation;

public class CreateWorkStationDTO
{
    public string Name { get; set; } = default!;

    public int Days { get; set; } = 1;

    public bool Active { get; set; }
}
