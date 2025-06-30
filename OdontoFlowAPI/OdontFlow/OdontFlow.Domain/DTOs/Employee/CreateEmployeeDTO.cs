namespace OdontFlow.Domain.DTOs.Employee;

public class CreateEmployeeDTO
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool ApplyCommission { get; set; }
    public bool IsSalesRepresentative { get; set; }
    public float CommissionPercentage { get; set; }

    public List<CreateClientCommissionDTO> ClientCommissions { get; set; } = new();
}

public class CreateClientCommissionDTO
{
    public Guid ClientId { get; set; }
    public float CommissionPercentage { get; set; }
}
