using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;  
using Model = OdontFlow.Domain.Entities.Order;
using EmployeeModel = OdontFlow.Domain.Entities.Employee;

namespace OdontFlow.Application.BussinesProcess.Reports.Query;

public class GetEmployeeComisionQuery(string OrderNumber)
{
    public string OrderNumber { get; set; } = OrderNumber;
}

public class GetEmployeeComisionQueryHandler(
    IReadOnlyRepository<Guid, Model> orderRepo,
    IReadOnlyRepository<Guid, EmployeeModel> employeeRepo
) : IQueryHandler<GetEmployeeComisionQuery, IEnumerable<object>>
{
    public async Task<IEnumerable<object>> Handle(GetEmployeeComisionQuery query)
    {
        var orders = await orderRepo.GetAllMatchingAsync(
            o => o.Barcode == query.OrderNumber || query.OrderNumber == "" || query.OrderNumber == null, 
            new[] { "Client", "Items.Product" });

        var employees = await employeeRepo.GetAllMatchingAsync(
            e => e.ApplyCommission && e.Active && !e.Deleted,
            new[] { "Clients" });

        var result = orders.Select(order =>
        {
            var estado = order.PaymentComplete ? "PAGADO" : "POR PAGAR";

            var baseData = new Dictionary<string, object>
            {
                ["GrupoTrabajo"] = ((LIST_CATEGORY) order.Client.GroupId).ToString(),
                ["Orden"] = order.Barcode,
                ["Cantidad"] = order.Items.Sum(i => i.Quantity),
                ["Producto"] = string.Join(", ", order.Items.Select(i => i.Product.Name)),
                ["FechaAlta"] = order.CreationDate.ToString("dd/MM/yyyy"),

                ["Cliente"] = order.Client.Name,
                ["Paciente"] = order.PatientName,
                ["CostoTotal"] = order.Cost + order.Tax,
                ["TipoPedido"] = order.OrderTypeId.ToString()  ,
                ["Estado"] = estado
            };

            foreach (var emp in employees)
            {
                var key = $"{emp.Name}";
                var isClient = emp.Clients.Any(c => c.Id == order.ClientId);

                if (isClient)
                {
                    var porcentaje = emp.CommissionPercentage;
                    var monto = Math.Round((order.Cost + order.Tax) * (decimal)porcentaje, 2);
                    baseData[$"{key} %"] = $"{porcentaje:P0}";
                    baseData[$"{key} Monto"] = monto;
                }
                else
                {
                    baseData[$"{key} %"] = "-";
                    baseData[$"{key} Monto"] = 0;
                }
            }

            return baseData;
        });

        return result.ToList();
    }
}
