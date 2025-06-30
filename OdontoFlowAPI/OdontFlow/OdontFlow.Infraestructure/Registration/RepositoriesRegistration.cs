using OdontFlow.Infrastructure.Repositories.Base;
using Microsoft.Extensions.DependencyInjection; 
using OdontFlow.Domain.Repositories.Base;
using OdontFlow.Infraestructure.Authentication;

namespace OdontFlow.Infrastructure.Registration;

public static class RepositoriesRegistration
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped(typeof(IReadOnlyRepository<,>), typeof(ReadOnlyRepository<,>));
        services.AddScoped(typeof(IWriteOnlyRepository<,>), typeof(WriteOnlyRepository<,>));

        return services;
    }
}
