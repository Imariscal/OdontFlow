
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OdontFlow.Application.BussinesProcess.Auth.Command;
using OdontFlow.Application.BussinesProcess.Auth.Query;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Application.BussinesProcess.Client.Command;
using OdontFlow.Application.BussinesProcess.Employee.Command;
using OdontFlow.Application.BussinesProcess.Employee.Query;
using OdontFlow.Application.BussinesProcess.PriceList.Command;
using OdontFlow.Application.BussinesProcess.PriceList.Query;
using OdontFlow.Application.BussinesProcess.PriceListItem.Command;
using OdontFlow.Application.BussinesProcess.Product.Command;
using OdontFlow.Application.BussinesProcess.Product.Query;
using OdontFlow.Application.BussinesProcess.Supplier.Command;
using OdontFlow.Application.BussinesProcess.Supplier.Query;
using OdontFlow.Application.BussinesProcess.WorkPlan.Command;
using OdontFlow.Application.BussinesProcess.WorkPlan.Query;
using OdontFlow.Application.BussinesProcess.WorkStation.Command;
using OdontFlow.Application.BussinesProcess.WorkStation.Query;
using OdontFlow.Domain.BusinessRules.PriceList;
using OdontFlow.Domain.BusinessRules.Product;
using OdontFlow.Domain.BusinessRules.Supplier;
using OdontFlow.Domain.BusinessRules.User;
using OdontFlow.Domain.DTOs;
using OdontFlow.Domain.DTOs.PriceList;
using OdontFlow.Domain.DTOs.Product;
using OdontFlow.Domain.DTOs.Supplier;
using OdontFlow.Domain.Entities;
using OdontFlow.Domain.ViewModel.Client;
using OdontFlow.Domain.ViewModel.Employee;
using OdontFlow.Domain.ViewModel.PriceList;
using OdontFlow.Domain.ViewModel.Product;
using OdontFlow.Domain.ViewModel.Supplier;
using OdontFlow.Domain.ViewModel.WorkPlan;
using OdontFlow.Domain.ViewModel.WorkStation;

using OdontFlow.Application.BussinesProcess.Order.Command;
using OdontFlow.Application.BussinesProcess.Order.Query;
using OdontFlow.Domain.ViewModel.Order;
using OdontFlow.CrossCutting.Common;
using OdontFlow.Application.BussinesProcess.OrderPayment.Query;
using OdontFlow.Application.BussinesProcess.OrderPayment.Command;
using OdontFlow.Domain.ViewModel.OrderPayment;
using OdontFlow.Application.BussinesProcess.StationWork.Query;
using OdontFlow.Domain.ViewModel.Lab;
using OdontFlow.Application.BussinesProcess.Lab.Query;
using OdontFlow.Domain.ViewModel.StationWork;
using OdontFlow.Application.BussinesProcess.Lab.Command;
using OdontFlow.Domain.ViewModel.User;
using OdontFlow.Domain.ViewModel.Report;
using OdontFlow.Application.BussinesProcess.Reports.Query;

namespace OdontFlow.Application.Registration
{
    public static class BusinessProcessRegistration
    {
        public static IServiceCollection RegisterBusinessProcess(this IServiceCollection services)
        {
            // User
            services.AddTransient<IQueryHandler<GetUserByEmailQuery, User>, GetUserByEmailQueryHandler>();
            services.AddTransient<IQueryHandler<GetUsersQuery, IEnumerable<UserViewModel>>, GetUsersQueryHandler>();
            services.AddTransient<ICommandHandler<CreateRegisterCommand, User>, CreateRegisterCommandHandler>();
            services.AddTransient<ICommandHandler<ChangeFirstTimePasswordCommand, User>, ChangeFirstTimePasswordCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateRegisterCommand, UserViewModel>, UpdateRegisterCommandHandler>();

            // User Validator
            services.AddTransient<IValidator<RegisterRequest>, RegisterUserValidator>();
            services.AddTransient<IValidator<ResetFirstPasswordRequest>, ChangeFirstTimePasswordValidator>();

            //Products
            services.AddTransient<IQueryHandler<GetProductsQuery, IEnumerable<ProductViewModel>>, GetProductsQueryHandler>();
            services.AddTransient<IQueryHandler<GetProductsByCategoryQuery, IEnumerable<ProductViewModel>>, GetProductsByCategoryQueryHandler>();
            services.AddTransient<IQueryHandler<GetProductsWithoutPlanQuery, IEnumerable<ProductViewModel>>, GetProductsWithoutPlanQueryHandler>();
            services.AddTransient<IQueryHandler<GetProductsByPlanIdQuery, IEnumerable<ProductViewModel>>, GetProductsByPlanIdQueryHandler>();

            services.AddTransient<IQueryHandler<GetPriceListItemsQuery, IEnumerable<PriceListItemViewModel>>, GetPriceListItemsQueryHandler>();
            services.AddTransient<ICommandHandler<CreateProductCommand, ProductViewModel>, CreateProductCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateProductCommand, ProductViewModel>, UpdateProductCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteProductCommand, ProductViewModel>, DeleteProductCommandHandler>();

            //Products Validator
            services.AddTransient<IValidator<CreateProductDTO>, CraeateProductValidator>();
            services.AddTransient<IValidator<UpdateProductDTO>, UpdateProductValidator>();

            //Supplier
            services.AddTransient<IQueryHandler<GetSuppliersQuery, IEnumerable<SupplierViewModel>>, GetSuppliersQueryHandler>();
            services.AddTransient<ICommandHandler<CreateSupplierCommand, SupplierViewModel>, CreateSupplierCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateSupplierCommand, SupplierViewModel>, UpdateSupplierCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteSupplierCommand, SupplierViewModel>, DeleteSupplierCommandHandler>();

            //Supplier Validator
            services.AddTransient<IValidator<CreateSupplierDTO>, CreateSupplierValidator>();
            services.AddTransient<IValidator<UpdateSupplierDTO>, UpdateSupplierValidator>();


            //PriceList
            services.AddTransient<IQueryHandler<GetPriceListsQuery, IEnumerable<PriceListViewModel>>, GetPriceListsQueryHandler>();
            services.AddTransient<ICommandHandler<CreatePriceListCommand, PriceListViewModel>, CreatePriceListCommandHandler>();
            services.AddTransient<ICommandHandler<UpdatePriceListCommand, PriceListViewModel>, UpdatePriceListCommandHandler>();
            services.AddTransient<ICommandHandler<DeletePriceListCommand, PriceListViewModel>, DeletePriceListCommandHandler>();
            services.AddTransient<ICommandHandler<UpdatePriceListItemCommand, PriceListItemViewModel>, UpdatePriceListItemCommandHandler>();
            services.AddTransient<ICommandHandler<CreatePriceListItemCommand, PriceListItemViewModel>, CreatePriceListItemCommandHandler>();
            services.AddTransient<ICommandHandler<DeletePriceListItemCommand, PriceListItemViewModel>, DeletePriceListItemCommandHandler>();
            //Supplier Validator
            services.AddTransient<IValidator<CreatePriceListDTO>, CreatePriceListValidator>();
            services.AddTransient<IValidator<UpdatePriceListDTO>, UpdatePriceListValidator>();

            //Client
            services.AddTransient<IQueryHandler<GetClientsQuery, IEnumerable<ClientViewModel>>, GetClientsQueryHandler>();
            services.AddTransient<IQueryHandler<GetClientsActiveQuery, IEnumerable<ClientViewModel>>, GetClientsActiveQueryHandler>();
            services.AddTransient<ICommandHandler<CreateClientCommand, ClientViewModel>, CreateClientCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateClientCommand, ClientViewModel>, UpdateClientCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteClientCommand, ClientViewModel>, DeleteClientCommandHandler>();

            //WorkStation
            services.AddTransient<IQueryHandler<GetWorkStationsQuery, IEnumerable<WorkStationViewModel>>, GetWorkStationsQueryHandler>();
            services.AddTransient<IQueryHandler<GetWorkStationsActiveQuery, IEnumerable<WorkStationViewModel>>, GetWorkStationsActiveQueryHandler>();
            services.AddTransient<ICommandHandler<CreateWorkStationCommand, WorkStationViewModel>, CreateWorkStationCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateWorkStationCommand, WorkStationViewModel>, UpdateWorkStationCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteWorkStationCommand, WorkStationViewModel>, DeleteWorkStationCommandHandler>();

            //WorkPlan
            services.AddTransient<IQueryHandler<GetWorkPlansQuery, IEnumerable<WorkPlanViewModel>>, GetWorkPlansQueryHandler>();
            services.AddTransient<ICommandHandler<CreateWorkPlanCommand, WorkPlanViewModel>, CreateWorkPlanCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateWorkPlanCommand, WorkPlanViewModel>, UpdateWorkPlanCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteWorkPlanCommand, WorkPlanViewModel>, DeleteWorkPlanCommandHandler>();
           
            // Employee
            services.AddTransient<IQueryHandler<GetEmployeesQuery, IEnumerable<EmployeeViewModel>>, GetEmployeesQueryHandler>();
            services.AddTransient<IQueryHandler<GetEmployeesActiveQuery, IEnumerable<EmployeeViewModel>>, GetEmployeesActiveQueryHandler>();
            services.AddTransient<IQueryHandler<GetSalesEmployeesActiveQuery, IEnumerable<EmployeeViewModel>>, GetSalesEmployeesActiveQueryHandler>();
            services.AddTransient<ICommandHandler<CreateEmployeeCommand, EmployeeViewModel>, CreateEmployeeCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateEmployeeCommand, EmployeeViewModel>, UpdateEmployeeCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteEmployeeCommand, EmployeeViewModel>, DeleteEmployeeCommandHandler>();

            // Orders
            services.AddTransient<IQueryHandler<GetOrdersQuery, PagedResult<OrderViewModel>>, GetOrdersQueryHandler>();
            services.AddTransient<IQueryHandler<GetOrderByIdQuery, OrderViewModel>, GetOrderByIdQueryHandler>();
            services.AddTransient<IQueryHandler<GetOrdersByFilterQuery, PagedResult<OrderViewModel>>, GetOrdersByFilterQueryHandler>();

            services.AddTransient<ICommandHandler<CreateOrderCommand, OrderViewModel>, CreateOrderCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateOrderCommand, OrderViewModel>, UpdateOrderCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteOrderCommand, OrderViewModel>, DeleteOrderCommandHandler>();
            services.AddTransient<ICommandHandler<ConfirmOrderCommand, OrderViewModel>, ConfirmOrderCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteOrderItemCommand, OrderViewModel>, DeleteOrderItemCommandHandler>();
            services.AddTransient<ICommandHandler<DeliveryOrderCommand, OrderViewModel>, DeliveryOrderCommandHandler>();
            services.AddTransient<ICommandHandler<CreateOrderSecuenceCommand, OrderSequence>, CreateOrderSecuenceCommandHandler>();

            // Orders Payments
            services.AddTransient<IQueryHandler<GetOrderPaymentsQuery, IEnumerable<OrderPaymentViewModel>>, GetOrderPaymentsQueryHandler>();          

            services.AddTransient<ICommandHandler<CreateOrderPaymentCommand, OrderPaymentViewModel>, CreateOrderPaymentCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateOrderPaymentCommand, OrderPaymentViewModel>, UpdateOrderPaymentCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteOrderPaymentCommand, OrderPaymentViewModel>, DeleteOrderPaymentCommandHandler>();

            //Dashboard LAB
            services.AddTransient<IQueryHandler<GetStationWorkSummaryQuery, StationWorkSummaryViewModel>, GetStationWorkSummaryQueryHandler>();
            services.AddTransient<IQueryHandler<GetStationWorkDetailsQuery, IEnumerable<StationWorkDetailViewModel>>, GetStationWorkDetailsQueryHandler>();
            services.AddTransient<IQueryHandler<GetStationWorkDetailsByOrderQuery, IEnumerable<StationWorkDetailViewModel>>, GetStationWorkDetailsByOrderQueryHanlder>();

            services.AddTransient<ICommandHandler<ProcessOrderCommand, StationWorkDetailViewModel>, ProcessOrderCommandHandler>();
            services.AddTransient<ICommandHandler<CompleteOrderCommand, StationWorkDetailViewModel>, CompleteOrderCommandHandler>();
            services.AddTransient<ICommandHandler<RejectOrderCommand, StationWorkDetailViewModel>, RejectOrderCommandHandler>();
            services.AddTransient<ICommandHandler<BlockOrderCommand, StationWorkDetailViewModel>, BlockOrderCommandHandler>();
            services.AddTransient<ICommandHandler<UnBlockOrderCommand, StationWorkDetailViewModel>, UnBlockOrderCommandHandler>();

            //Reports  GetWorkedPiecesReportQuery
            services.AddTransient<IQueryHandler<GetFilteredProductivityReportQuery, ProductivityReportViewModel>, GetFilteredProductivityReportQueryHandler>(); 
            services.AddTransient<IQueryHandler<GetOrdersByAdvancedFilterQuery, PagedResult<OrderViewModel>>, GetOrdersByAdvancedFilterQueryHandler>();
            services.AddTransient<IQueryHandler<GetOrdersPaymentsByAdvancedFilterQuery, PagedResult<OrderViewModel>>, GetOrdersPaymentsByAdvancedFilterQueryHandler>();
            services.AddTransient<IQueryHandler<GetDebtOrdersQuery, PagedResult<OrderViewModel>>, GetDebtOrdersQueryHandler>();
            services.AddTransient<IQueryHandler<GetWorkedPiecesReportQuery, PagedResult<WorkedPiecesReportViewModel>>, GetWorkedPiecesReportQueryHandler>();
            services.AddTransient<IQueryHandler<GetPaymentAndDebtsDetailsQuery, PagedResult<ClientOrdersSummaryViewModel>>, GetPaymentAndDebtsDetailsQueryHandler>();
            services.AddTransient<IQueryHandler<GetCommissionOrdersReportQuery, CommissionOrdersReportViewModel>, GetCommissionOrdersReportQueryHandler>();
            services.AddTransient<IQueryHandler<GetDebtOrdersWithSummaryQuery, PagedWithSummaryResult<OrderViewModel>>, GetDebtOrdersWithSummaryQueryHandler>();
            services.AddTransient<IQueryHandler<GetDebtOrdersExportQuery, List<OrderViewModel>>, GetDebtOrdersExportQueryHandler>();
            return services;
        }
    }
}
