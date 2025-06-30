using Mapster;
using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using OdontFlow.Domain.ViewModel.Order; 
using Model = OdontFlow.Domain.Entities.Order;

namespace OdontFlow.Application.BussinesProcess.Reports.Query;

public class GetCommissionOrdersReportQuery(OrderAdvancedFilterViewModel input)
{
    public OrderAdvancedFilterViewModel Input { get; set; } = input;
}
public class GetCommissionOrdersReportQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper
) : IQueryHandler<GetCommissionOrdersReportQuery, CommissionOrdersReportViewModel>
{
    public async Task<CommissionOrdersReportViewModel> Handle(GetCommissionOrdersReportQuery queryData)
    {
        var query = queryData.Input;

        var orders = await readOnlyRepository.GetAllMatchingAsync(
            x =>
                (string.IsNullOrEmpty(query.Search) ||
                    x.PatientName.Contains(query.Search) ||
                    x.RequesterName.Contains(query.Search) ||
                    x.Barcode.Contains(query.Search) ||
                    x.Client.Employee.Name.Contains(query.Search) ||
                    x.Client!.Name.Contains(query.Search)) &&
                (!query.OrderStatusId.HasValue || x.OrderStatusId == query.OrderStatusId.Value) &&
                (!query.OrderTypeId.HasValue || x.OrderTypeId == query.OrderTypeId) &&
                (!query.GroupId.HasValue || x.Client.GroupId == query.GroupId) &&
                (!query.CreationDateStart.HasValue || x.CreationDate >= query.CreationDateStart.Value) &&
                (!query.CreationDateEnd.HasValue || x.CreationDate <= query.CreationDateEnd.Value) &&
                x.PaymentComplete == true,
            "Client.Employee",
            "Items.Product"
        );

        var detailItems = new List<CommissionOrderDetailItem>();

        foreach (var order in orders)
        {
            var client = order.Client;
            var employee = client.Employee;

            if (employee == null)
                continue;

            var total = order.Items.Sum(i => i.Quantity * i.UnitCost);
            var commissionPercentage = order.CommissionPercentage;
            var commissionAmount = total * commissionPercentage / 100;

            detailItems.Add(new CommissionOrderDetailItem
            {
                EmployeeName = employee.Name,
                OrderBarcode = order.Barcode,
                ClientName = client.Name,
                CreationDate = order.CreationDate,
                OrderTotal = total,
                Order = mapper.Map<OrderViewModel>(order),
                CommissionPercentage = commissionPercentage,
                CommissionAmount = commissionAmount
            });
        }

        var pagedItems = detailItems
            .OrderByDescending(x => x.CreationDate)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        // Agrupamos para el resumen por empleado
        var summary = detailItems
            .GroupBy(x => x.EmployeeName)
            .Select(g => new CommissionEmployeeSummary
            {
                EmployeeName = g.Key,
                TotalOrders = g.Count(),
                TotalAmount = g.Sum(x => x.OrderTotal),
                TotalCommission = g.Sum(x => x.CommissionAmount)
            })
            .ToList();

        return new CommissionOrdersReportViewModel
        {
            Items = pagedItems,
            Summary = summary,
            TotalRecords = detailItems.Count
        };
    }
}
