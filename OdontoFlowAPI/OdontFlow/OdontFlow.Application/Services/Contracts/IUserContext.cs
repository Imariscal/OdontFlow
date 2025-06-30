namespace OdontFlow.Application.Services.Contracts;

public interface IUserContext
{
    Guid UserId { get; }

    Guid EmployeeId { get; }
    string Email { get; }
    string Role { get; }
}
