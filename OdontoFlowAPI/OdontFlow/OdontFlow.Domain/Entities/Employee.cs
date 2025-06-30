using OdontFlow.Domain.Entities.Base;

namespace OdontFlow.Domain.Entities;

public class Employee : Auditable<Guid>
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool ApplyCommission { get; set; }
    public bool IsSalesRepresentative { get; set; }
    public float CommissionPercentage { get; set; }  
 }
