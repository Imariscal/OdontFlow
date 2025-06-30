using OdontFlow.Domain.Entities.Base;

namespace OdontFlow.Domain.Entities;

public class StationWorkComments : Auditable<Guid>
{
    public Guid StationWorkId { get; set; } 
    public string Comment { get; set; }
}
