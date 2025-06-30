
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using OdontFlow.Infrastructure.Registration;
using OdontFlow.Domain.BusinessRules.Base;
using OdontFlow.Catalogos.Retail.Application.Services.Base;
using OdontFlow.Retail.Domain.BusinessRules.Base;
using OdontFlow.Application.Services.Contracts;
using OdontFlow.Application.Services;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Catalogos.Retail.Application.BusinessProcess.Base;
using OdontFlow.Application.BussinesProcess;
using Microsoft.Extensions.Configuration;

namespace OdontFlow.Application.Registration;
public static class ServiceAppRegister
{
    public static IServiceCollection RegisterAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        //  Mapper
        services.AddMapster();

        MapsterConfig.RegisterMappings();
        //  Register contexts and repositories
        services.RegisterContext(configuration);
        services.RegisterRepositories();
        
        services.AddScoped<IMediator, Mediator>();
        services.AddTransient(typeof(IValidationStrategy<>), typeof(FluentValidationStrategy<>));

        // Business Process
        services.RegisterBusinessProcess();

        //  Application services registration
        services.AddTransient(typeof(IBaseCrudApplicationService<,>), typeof(BaseCrudApplicationService<,>));
        services.AddTransient<IAuthService, AuthService>();

        //Products 
        services.AddTransient<IProductsService, ProductsService>();

        //Supplier 
        services.AddTransient<ISupplierService, SupplierService>();

        //PriceList 
        services.AddTransient<IPriceListService, PriceListService>();

        //Client
        services.AddTransient<IClientService, ClientService>();

        //Workstation
        services.AddTransient<IWorkStationService, WorkStationService>();

        //Workstation Plan
        services.AddTransient<IWorkPlanService, WorkPlanService>();

        //Empleados
        services.AddTransient<IEmployeeService, EmployeeService>();

        //Order
        services.AddTransient<IOrderService, OrderService>();

        //IOrderPaymentService
        services.AddTransient<IOrderPaymentService, OrderPaymentService>();

        //Lab
        services.AddTransient<ILabService, LabService>();

        //User
        services.AddScoped<IUserContext, UserContext>();

        //ReportService
        services.AddScoped<IReportService, ReportService>();

        //ReportService
        services.AddScoped<IOrderSequenceService, OrderSequenceService>();

        return services;
    }
}
