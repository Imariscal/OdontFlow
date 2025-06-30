using Mapster;
using OdontFlow.Domain.DTOs.Supplier;
using OdontFlow.Domain.ViewModel.Product;
using OdontFlow.Domain.ViewModel.Supplier;
using ProductModel = OdontFlow.Domain.Entities.Product;
using OrderPaymentModel = OdontFlow.Domain.Entities.OrderPayment;
using SupplierModel = OdontFlow.Domain.Entities.Supplier;
using PriceListModel = OdontFlow.Domain.Entities.PriceList;
using PriceListItemModel = OdontFlow.Domain.Entities.PriceListItem;
using ClientModel = OdontFlow.Domain.Entities.Client;
using WorkPlanModel = OdontFlow.Domain.Entities.WorkPlan;
using EmployeeModel = OdontFlow.Domain.Entities.Employee;
using StationWorkModel = OdontFlow.Domain.Entities.StationWork;
using OdontFlow.Domain.ViewModel.PriceList;
using OdontFlow.Domain.DTOs.PriceList;
using OdontFlow.Domain.DTOs.Client;
using OdontFlow.Domain.ViewModel.Client;
using OdontFlow.Domain.Entities;
using OdontFlow.Domain.DTOs.WorksPlan;
using OdontFlow.Domain.ViewModel.WorkPlan;
using OdontFlow.Domain.DTOs.Employee;
using OdontFlow.Domain.ViewModel.Employee;

using OrderModel = OdontFlow.Domain.Entities.Order;
using OrderItemModel = OdontFlow.Domain.Entities.OrderItem;
using OrderViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;
using OrderItemViewModel = OdontFlow.Domain.ViewModel.Order.OrderItemViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.Order.CreateOrderDTO;
using UpdateDTO = OdontFlow.Domain.DTOs.Order.UpdateOrderDTO;
using OdontFlow.Domain.DTOs.OrderPayment;
using OdontFlow.Domain.ViewModel.OrderPayment;
using OdontFlow.Domain.ViewModel.User;
using OdontFlow.Domain.DTOs.User;
using OdontFlow.Domain.ViewModel.StationWork;
using OdontFlow.Domain.ViewModel.Order;

namespace OdontFlow.Application.BussinesProcess;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        // ProductDto → ProductViewModel
        TypeAdapterConfig<ProductModel, ProductViewModel>.NewConfig()
            .Map(dest => dest.ProductCategory, src => src.ProductCategory.ToString())
            .Map(dest => dest.ProductCategoryId, src => src.ProductCategory)
            .Map(dest => dest.PriceFormatted, src => $"${src.Price:N2}");

        TypeAdapterConfig<CreateSupplierDTO, SupplierModel>.NewConfig();
        TypeAdapterConfig<UpdateSupplierDTO, SupplierModel>.NewConfig()
            .Map(dest => dest.Id, src => src.Id);

        TypeAdapterConfig<SupplierModel, SupplierViewModel>.NewConfig()
            .Map(dest => dest.CreditFormatted, src => $"${src.Credit:N2}");

        TypeAdapterConfig<PriceListModel, PriceListViewModel>.NewConfig()
            .Map(dest => dest.CategoryId, src => src.Category)
            .Map(dest => dest.Category, src => src.Category.ToString());

        TypeAdapterConfig<PriceListItemModel, PriceListItemViewModel>.NewConfig()
           .Map(dest => dest.PriceListName, src => src.PriceList.Name);

        TypeAdapterConfig<PriceListItemCreateDTO, PriceListItemModel>.NewConfig();

        TypeAdapterConfig<PriceListItemUpdateDTO, PriceListItemModel>.NewConfig()
            .Ignore(dest => dest.Id);

        TypeAdapterConfig<ClientInvoiceDTO, ClientInvoice>.NewConfig();
        TypeAdapterConfig<ClientInvoice, ClientInvoiceViewModel>.NewConfig();

        TypeAdapterConfig<CreateClientDTO, ClientModel>.NewConfig()
            .Map(dest => dest.ClientInvoice, src => src.ClientInvoice);


        TypeAdapterConfig<UpdateClientDTO, ClientModel>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.ClientInvoice, src => src.ClientInvoice);


        TypeAdapterConfig<CreateWorkPlanDTO, WorkPlanModel>.NewConfig()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Stations, src => src.Stations);

        TypeAdapterConfig<WorkStationPlan, WorkStationPlanViewModel>.NewConfig()
        .Map(dest => dest.WorkStationId, src => src.WorkStationId)
        .Map(dest => dest.WorkStationName, src => src.WorkStation.Name)
        .Map(dest => dest.Order, src => src.Order);

        TypeAdapterConfig<WorkPlanModel, WorkPlanViewModel>.NewConfig()
            .Map(dest => dest.Stations, src => src.Stations.Adapt<List<WorkStationPlanViewModel>>())
            .Map(dest => dest.Products, src => src.Products.Adapt<List<WorkPlanProductViewModel>>());

        TypeAdapterConfig<CreateEmployeeDTO, EmployeeModel>.NewConfig();
        TypeAdapterConfig<UpdateEmployeeDTO, EmployeeModel>.NewConfig();

        TypeAdapterConfig<EmployeeModel, EmployeeViewModel>
            .NewConfig();

        TypeAdapterConfig<ClientInvoice, ClientInvoiceViewModel>.NewConfig();
 

        TypeAdapterConfig<ClientModel, ClientViewModel>.NewConfig()
            .Map(dest => dest.ClientInvoice, src => src.ClientInvoice)
            .Map(dest => dest.EmployeeName, src => src.Employee != null ? src.Employee.Name : null)
            .Map(dest => dest.WorkGroup, src => ((LIST_CATEGORY)src.GroupId).ToString())
            .Map(dest => dest.PriceList, src => src.PriceList != null ? src.PriceList.Name : null)
            .Map(dest => dest.PriceListId, src => (Guid?)src.PriceListId)
            .Map(dest => dest.EmployeeId, src => src.EmployeeId)
            .Map(dest => dest.CommissionPercentage, src => src.CommissionPercentage)
            .Ignore(dest => dest.Client)
            .Ignore(dest => dest.SalesEmployee)
            .IgnoreNullValues(true);
        // Order → ViewModel
        TypeAdapterConfig<OrderModel, OrderViewModel>.NewConfig()
            .Map(dest => dest.ClientName, src => src.Client != null ? src.Client.Name : null)
            .Map(dest => dest.Barcode, src => src.Barcode)
            .Map(dest => dest.OrderStatus, src => ((ORDER_STATUS)src.OrderStatusId).ToString())
            .Map(dest => dest.OrderType, src => ((ORDER_TYPE)src.OrderTypeId).ToString())
            .Map(dest => dest.Items, src => src.Items != null
                ? src.Items.Where(i => i != null).Adapt<List<OrderItemViewModel>>()
                : new List<OrderItemViewModel>())
            .Map(dest => dest.Payments, src => src.Payments != null
                ? src.Payments.Where(p => p != null).Adapt<List<OrderPaymentViewModel>>()
                : new List<OrderPaymentViewModel>())
            .Map(dest => dest.Client, src => src.Client)
            .Map(dest => dest.WorkGroup, src => src.Client != null
                ? ((LIST_CATEGORY)src.Client.GroupId).ToString()
                : null)
            .Map(dest => dest.CurrentStationWork, src =>
                src.StationWorks != null
                    ? src.StationWorks
                        .Where(sw => sw != null && sw.WorkStatus == WORK_STATUS.PROCESO || sw.WorkStatus == WORK_STATUS.TERMINADO) // Solo el que esté en proceso
                        .Select(sw => new StationWorkCurrentViewModel
                        {
                            WorkStationName = sw.WorkStation != null ? sw.WorkStation.Name : "",
                            EmployeeStartDate = sw.EmployeeStartDate,
                            EmployeeName =  sw.Employee != null ? sw.Employee.Name : "", 
                        })
                        .FirstOrDefault() // Solo uno
                    : null
            );


        // OrderItem → ViewModel
        TypeAdapterConfig<OrderItemModel, OrderItemViewModel>.NewConfig()
            .Map(dest => dest.ProductName, src => src.Product != null ? src.Product.Name : null);

        // CreateDTO → Order
        TypeAdapterConfig<CreateDTO, OrderModel>.NewConfig()
            .Ignore(dest => dest.Items); // se asignan manualmente en el handler

        // UpdateDTO → Order
        TypeAdapterConfig<UpdateDTO, OrderModel>.NewConfig()
            .Ignore(dest => dest.Items); // también se asignan manualmente

        TypeAdapterConfig<CreateOrderPaymentDTO, OrderPaymentModel>.NewConfig();
        TypeAdapterConfig<UpdateOrderPaymentDTO, OrderPaymentModel>.NewConfig();
        TypeAdapterConfig<OrderPaymentModel, OrderPaymentViewModel>.NewConfig()
                 .Map(dest => dest.PaymentType, src => ((PAYMENT_TYPE)src.PaymentTypeId).ToString())
                 .Map(dest => dest.Barcode, src => src.Order.Barcode)
                 .Map(dest => dest.ClientName, src => src.Order.Client.Name)
                 .Map(dest => dest.PatientName, src => src.Order.PatientName);


        TypeAdapterConfig<User, UserViewModel>.NewConfig()
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.RoleName, src => src.Role.ToString())
            .Map(dest => dest.RoleId, src => src.Role)
            .Map(dest => dest.ChangePassword, src => src.ChangePassword)
            .Map(dest => dest.EmployeeId, src => src.EmployeeId)
            .Map(dest => dest.EmployeeName, src => src.Employee != null ? src.Employee.Name : null)
                        .Map(dest => dest.ClientId, src => src.ClientId)
            .Map(dest => dest.ClientName, src => src.Client != null ? src.Client.Name : null)
            .Map(dest => dest.Id, src => src.Id);

        TypeAdapterConfig<CreateUserDTO, User>.NewConfig()
    .Map(dest => dest.Email, src => src.Email)
    .Map(dest => dest.Role, src => src.Role)
    .Map(dest => dest.ChangePassword, src => src.ChangePassword)
    .Map(dest => dest.EmployeeId, src => src.EmployeeId);

        TypeAdapterConfig<UpdateUserDTO, User>.NewConfig()
    .Map(dest => dest.Id, src => src.Id)
    .Map(dest => dest.Email, src => src.Email)
    .Map(dest => dest.Role, src => src.Role)
    .Map(dest => dest.ChangePassword, src => src.ChangePassword)
    .Map(dest => dest.EmployeeId, src => src.EmployeeId);

        TypeAdapterConfig<StationWorkModel, StationWorkDetailViewModel>.NewConfig()
            .Map(dest => dest.StationWorkId, src => src.Id)
            .Map(dest => dest.OrderNumber, src => src.Barcode)
            .Map(dest => dest.WorkStationName, src => src.WorkStation != null ? src.WorkStation.Name : "")
            .Map(dest => dest.ProductName, src => src.Product != null ? src.Product.Name : "")
            .Map(dest => dest.OrderColor, src => src.Order != null ? src.Order.Color : "")
            .Map(dest => dest.ClientName, src => src.Order != null && src.Order.Client != null ? src.Order.Client.Name : "")
            .Map(dest => dest.StationStartDate, src => src.StationStartDate)
            .Map(dest => dest.StationEndDate, src => src.StationEndDate)
            .Map(dest => dest.EmployeeStartDate, src => src.EmployeeStartDate)
            .Map(dest => dest.EmployeeEndDate, src => src.EmployeeEndDate)
            .Map(dest => dest.WorkStatus, src => (int)src.WorkStatus)
            .Map(dest => dest.InProgress, src => src.InProgress)
            .Map(dest => dest.Step, src => src.Step)
            .Ignore(dest => dest.WorkStatusIndicator)
            .Ignore(dest => dest.WorkedOnTime)
            .Ignore(dest => dest.ProductivityPercent)
            .Ignore(dest => dest.Teeth)
            .Ignore(dest => dest.TeethDetails)
            .Ignore(dest => dest.PreviousStationName)
            .Ignore(dest => dest.PreviousEmployeeName)
            .Ignore(dest => dest.PreviousEndDate);


    }
}
