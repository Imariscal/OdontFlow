namespace OdontFlow.Domain.DTOs;
public class LoginRequest{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class AuthResult{
    public string Token { get; set; } = default!;
    public bool ChangePassword { get; set; } = default!;
}

public class RegisterRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public USER_ROLE Role { get; set; } = USER_ROLE.ADMIN;
    public Guid? EmployeeId { get; set; }
    public bool ChangePassword { get; set; }
    public bool Active { get; set; } = true;
}

public class ResetFirstPasswordRequest
{
    public string NewPassword { get; set; } = default!;
}