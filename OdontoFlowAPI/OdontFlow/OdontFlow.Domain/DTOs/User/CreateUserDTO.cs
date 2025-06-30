using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontFlow.Domain.DTOs.User;

public class CreateUserDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [StringLength(255)]
    public string Password { get; set; } = default!;

    public USER_ROLE Role { get; set; } = USER_ROLE.ADMIN;

    public bool ChangePassword { get; set; } = true;

    public Guid? EmployeeId { get; set; }

    public Guid? ClientId { get; set; }
}