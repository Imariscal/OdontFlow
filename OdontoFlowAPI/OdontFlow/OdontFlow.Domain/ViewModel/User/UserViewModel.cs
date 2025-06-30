namespace OdontFlow.Domain.ViewModel.User;

public class UserViewModel
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string RoleName { get; set;  }
    public int RoleId { get; set; }  
    public bool ChangePassword { get; set; }
    public string Password { get; set; }
    public Guid? EmployeeId { get; set; }
    public string? EmployeeName { get; set; } // ejemplo si quieres mostrar el nombre del empleado
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Active { get; set; }
    public string PasswordHash { get; set; }
    public Guid? ClientId { get; set; }
    public string? ClientName { get; set; }
}
