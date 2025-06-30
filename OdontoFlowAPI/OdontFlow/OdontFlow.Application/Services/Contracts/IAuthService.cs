using OdontFlow.Domain.DTOs;
using OdontFlow.Domain.DTOs.User;
using OdontFlow.Domain.ViewModel.User;
namespace OdontFlow.Application.Services.Contracts;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> ChangeInitialPassword(Guid Id, ResetFirstPasswordRequest request);
    Task<UserViewModel> UpdateAsync(UpdateUserDTO request);
    Task<IEnumerable<UserViewModel>> GetUsersAsync();
}