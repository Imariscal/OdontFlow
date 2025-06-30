using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Common;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Order;
using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;
using OdontFlow.Domain.ViewModel.Order; 

namespace OdontFlow.Application.BussinesProcess.Reports.Query;

public class GetOrdersPaymentsByAdvancedFilterQuery(OrderAdvancedFilterViewModel input)
{
    public OrderAdvancedFilterViewModel Input { get; set;} = input;
}


public class GetOrdersPaymentsByAdvancedFilterQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper
) : IQueryHandler<GetOrdersPaymentsByAdvancedFilterQuery, PagedResult<ViewModel>>
{

    public async Task<PagedResult<ViewModel>> Handle(GetOrdersPaymentsByAdvancedFilterQuery queryData)
    {
        var query = queryData.Input;

        var all = await readOnlyRepository.GetAllMatchingAsync(
            x =>
                x.Payments.Any() &&
                (string.IsNullOrEmpty(query.Search) ||
                    x.PatientName.Contains(query.Search) ||
                    x.RequesterName.Contains(query.Search) ||
                    x.Barcode.Contains(query.Search) ||
                    x.Client!.Name.Contains(query.Search)) &&
                (!query.GroupId.HasValue || x.Client.GroupId == query.GroupId) &&
                x.Payments.Any(p =>
                    (!query.PaymentTypeId.HasValue || p.PaymentTypeId == query.PaymentTypeId) &&
                    (!query.CreationDateStart.HasValue || p.CreationDate >= query.CreationDateStart.Value) &&
                    (!query.CreationDateEnd.HasValue || p.CreationDate <= query.CreationDateEnd.Value)
                    ),
                    "Client", "Items", "Items.Product", "Payments", "StationWorks", "StationWorks.WorkStation", "StationWorks.Employee", "Client.Employee"
        );

        // 👉 Aquí limpias los pagos para que solo dejes los filtrados:
        foreach (var order in all)
        {
            order.Payments = order.Payments
                .Where(p =>
                    (!query.PaymentTypeId.HasValue || p.PaymentTypeId == query.PaymentTypeId) &&
                    (!query.CreationDateStart.HasValue || p.CreationDate >= query.CreationDateStart.Value) &&
                    (!query.CreationDateEnd.HasValue || p.CreationDate <= query.CreationDateEnd.Value)
                )
                .ToList();
        }

        var total = all.Count();
        var skip = (query.Page - 1) * query.PageSize;

        var items = all
            .OrderByDescending(x => x.CreationDate)
            .Skip(skip)
            .Take(query.PageSize)
            .ToList();

        var mappedOrders = mapper.Map<IEnumerable<ViewModel>>(items);

        return new PagedResult<ViewModel>(
            mappedOrders,
            total,
            query.Page,
            query.PageSize
        );
    }

}
