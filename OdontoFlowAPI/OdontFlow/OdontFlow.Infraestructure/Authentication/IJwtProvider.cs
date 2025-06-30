using OdontFlow.Domain.Entities;

namespace OdontFlow.Infraestructure.Authentication;
public interface IJwtProvider
{
    string GenerateToken(User user);
}