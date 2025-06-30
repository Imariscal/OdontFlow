using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Common;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Order;
using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;
using OdontFlow.Domain.ViewModel.Order; 

namespace OdontFlow.Application.BussinesProcess.Reports.Query;

public class GetOrdersByAdvancedFilterQuery(OrderAdvancedFilterViewModel input)
{
    public OrderAdvancedFilterViewModel Input { get; set;} = input;
}


public class GetOrdersByAdvancedFilterQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper
) : IQueryHandler<GetOrdersByAdvancedFilterQuery, PagedResult<ViewModel>>
{
    public async Task<PagedResult<ViewModel>> Handle(GetOrdersByAdvancedFilterQuery queryData)
    {
        var query = queryData.Input;

        var all = await readOnlyRepository.GetAllMatchingAsync(
            x =>
                (string.IsNullOrEmpty(query.Search) ||
                    x.PatientName.Contains(query.Search) ||
                    x.RequesterName.Contains(query.Search) ||
                    x.Barcode.Contains(query.Search) ||
                    x.Client!.Name.Contains(query.Search)) &&
                (!query.OrderStatusId.HasValue || x.OrderStatusId == query.OrderStatusId.Value) &&
                (!query.OrderTypeId.HasValue || x.OrderTypeId == query.OrderTypeId) &&
                (!query.GroupId.HasValue || x.Client.GroupId == query.GroupId) &&
                (string.IsNullOrEmpty(query.ClientName) || x.Client!.Name.Contains(query.ClientName)) &&
                (string.IsNullOrEmpty(query.PatientName) || x.PatientName.Contains(query.PatientName)) &&
                (string.IsNullOrEmpty(query.RequesterName) || x.RequesterName.Contains(query.RequesterName)) &&
                (!query.CreationDateStart.HasValue || x.CreationDate >= query.CreationDateStart.Value) &&
                (!query.CreationDateEnd.HasValue || x.CreationDate <= query.CreationDateEnd.Value) &&
                (!query.CommitmentDateStart.HasValue || x.CommitmentDate >= query.CommitmentDateStart.Value) &&
                (!query.CommitmentDateEnd.HasValue || x.CommitmentDate <= query.CommitmentDateEnd.Value) &&
                (
                    (query.OrderStatusId == ORDER_STATUS.PAGADO && x.PaymentComplete == true) ||
                    (!query.OrderStatusId.HasValue && (!query.PaymentComplete.HasValue || x.PaymentComplete == query.PaymentComplete)) ||
                    (query.OrderStatusId != ORDER_STATUS.PAGADO && (!query.PaymentComplete.HasValue || x.PaymentComplete == query.PaymentComplete))
                ) &&
                (!query.ApplyInvoice.HasValue || x.ApplyInvoice == query.ApplyInvoice) &&
                (!query.MinBalance.HasValue || x.Balance >= query.MinBalance) &&
                (!query.MaxBalance.HasValue || x.Balance <= query.MaxBalance),
            "Client", "Items", "Payments", "StationWorks", "StationWorks.WorkStation", "StationWorks.Employee", "Items.Product", "Client.Employee"
        );

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
