using Microsoft.AspNetCore.Http;
using OdontFlow.Application.Services.Contracts;
using System.Security.Claims;

namespace OdontFlow.Application.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId =>
        Guid.Parse(_httpContextAccessor.HttpContext?.User?.Claims
    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
    ?? throw new UnauthorizedAccessException("UserId no encontrado."));

        public string Email =>
        _httpContextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
            ?? throw new UnauthorizedAccessException("Email no encontrado.");

    public string Role =>
        _httpContextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
            ?? throw new UnauthorizedAccessException("Role no encontrado.");

    public Guid EmployeeId =>
    Guid.Parse(_httpContextAccessor.HttpContext?.User?.Claims
.FirstOrDefault(c => c.Type == "EmployeeId")?.Value
?? throw new UnauthorizedAccessException("EmployeeId no encontrado."));
}

