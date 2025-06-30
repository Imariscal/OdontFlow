using Microsoft.Extensions.DependencyInjection;
using OdontFlow.Infrastructure.Repositories.Base;
using OdontFlow.Persistence.Contexts.Base;
using OdontFlow.Persistence.Contexts;
using OdontFlow.Persistence.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace OdontFlow.Infrastructure.Registration;

public static class ContextRegistration
{
    public static IServiceCollection RegisterContext(this IServiceCollection services, IConfiguration configuration)
    {
        var readOnlyConnection = configuration.GetConnectionString("ReadOnly");
        var writeOnlyConnection = configuration.GetConnectionString("WriteOnly");

        services.AddDbContext<ReadOnlyContext>(options =>
        {
            options.UseSqlServer(readOnlyConnection);
        });

        services.AddDbContext<WriteOnlyContext>(options =>
        {
            options.UseSqlServer(writeOnlyConnection);
            options.AddInterceptors(new WriteOnlyCommandInterceptor());
        });


        services.AddDbContext<ReadOnlyContext>();
        services.AddDbContext<WriteOnlyContext>(o => o.AddInterceptors(new WriteOnlyCommandInterceptor()));
        services.AddTransient<IReadOnlyContext, ReadOnlyContext>();
        services.AddScoped<IWriteOnlyContext, WriteOnlyContext>();
        services.AddScoped<IWriteOnlyUnitOfWork, WriteOnlyUnitOfWork>();
        services.AddScoped<IReadOnlyUnitOfWork, ReadOnlyUnitOfWork>();

        services.AddMemoryCache();
        return services;
    }
}
