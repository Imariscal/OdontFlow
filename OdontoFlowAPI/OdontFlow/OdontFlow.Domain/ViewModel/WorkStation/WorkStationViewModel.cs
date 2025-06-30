namespace OdontFlow.Domain.ViewModel.WorkStation;

public class WorkStationViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int Days { get; set; } = 1;
    public bool Active { get; set; }
}
