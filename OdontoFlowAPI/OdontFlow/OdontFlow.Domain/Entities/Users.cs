using OdontFlow.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
namespace OdontFlow.Domain.Entities;

public class User : Auditable<Guid>
{
    public string Email { get; set; } = default!;
    [StringLength(255)]
    public string PasswordHash { get; set; } = default!;
    public USER_ROLE Role { get; set; } = USER_ROLE.ADMIN;
    public bool ChangePassword { get; set; }
    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public Guid? ClientId { get; set; }
    public Client? Client { get; set; }
}
