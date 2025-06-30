using OdontFlow.Domain.Entities.Base;
namespace OdontFlow.Domain.Entities;

public class WorkingHour : Auditable<Guid>
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

public class Holiday : Auditable<Guid>
{ 
    public DateTime Date { get; set; }
    public string? Description { get; set; }
}
