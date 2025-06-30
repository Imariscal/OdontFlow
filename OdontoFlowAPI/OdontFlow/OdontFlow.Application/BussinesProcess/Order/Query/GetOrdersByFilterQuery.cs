using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Order;
using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;
using OdontFlow.CrossCutting.Common;

namespace OdontFlow.Application.BussinesProcess.Order.Query;

public class GetOrdersByFilterQuery
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetOrdersByFilterQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper
) : IQueryHandler<GetOrdersByFilterQuery, PagedResult<ViewModel>>
{
    public async Task<PagedResult<ViewModel>> Handle(GetOrdersByFilterQuery query)
    {
        var all = await readOnlyRepository.GetAllMatchingAsync(
            x =>
                string.IsNullOrEmpty(query.Search) ||
                x.PatientName.Contains(query.Search) ||
                x.RequesterName.Contains(query.Search),
         "Client", "Items", "Payments", "StationWorks", "StationWorks.WorkStation", "StationWorks.Employee" , "Items", "Items.Product"
        );

        var total = all.Count();
        var skip = (query.Page - 1) * query.PageSize;

        var items = all
            .Skip(skip)
            .Take(query.PageSize)
            .ToList();

        return new PagedResult<ViewModel>(
            mapper.Map<IEnumerable<ViewModel>>(items),
            total,
            query.Page,
            query.PageSize
        );
    }
}