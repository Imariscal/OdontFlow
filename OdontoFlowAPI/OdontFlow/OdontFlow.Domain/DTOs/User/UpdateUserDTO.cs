using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontFlow.Domain.DTOs.User;

public class UpdateUserDTO
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
    public USER_ROLE Role { get; set; }
    public bool ChangePassword { get; set; }
    public Guid? EmployeeId { get; set; }
    public Guid? ClientId { get; set; }
    public bool Active { get; set; }
}