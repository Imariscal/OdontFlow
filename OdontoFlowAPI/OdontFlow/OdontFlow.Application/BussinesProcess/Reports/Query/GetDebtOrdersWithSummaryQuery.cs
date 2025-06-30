using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Common;
using OdontFlow.Domain.Repositories.Base;
using OdontFlow.Domain.ViewModel.Order;
using OdontFlow.Domain.ViewModel.Report;
using Model = OdontFlow.Domain.Entities.Order;
using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;


namespace OdontFlow.Application.BussinesProcess.Reports.Query;

public class GetDebtOrdersWithSummaryQuery(OrderAdvancedFilterViewModel input)
{
    public OrderAdvancedFilterViewModel Input { get; set; } = input;
}

public class GetDebtOrdersWithSummaryQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper
) : IQueryHandler<GetDebtOrdersWithSummaryQuery, PagedWithSummaryResult<ViewModel>>
{
    public async Task<PagedWithSummaryResult<ViewModel>> Handle(GetDebtOrdersWithSummaryQuery queryData)
    {
        var query = queryData.Input;
        var today = DateTime.Now.Date;

        var all = await readOnlyRepository.GetAllMatchingAsync(
            x =>
                x.Payment < x.Total &&
                (string.IsNullOrEmpty(query.Search) ||
                    x.PatientName.Contains(query.Search) ||
                    x.Barcode.Contains(query.Search) ||
                    x.Client!.Name.Contains(query.Search)) &&
                (!query.GroupId.HasValue || x.Client.GroupId == query.GroupId) &&
                (!query.CreationDateStart.HasValue || x.CreationDate >= query.CreationDateStart.Value) &&
                (!query.CreationDateEnd.HasValue || x.CreationDate <= query.CreationDateEnd.Value),
            "Client",
            "Items", "Items.Product",
            "Payments",
            "StationWorks", "StationWorks.WorkStation", "StationWorks.Employee",
            "Client.Employee"
        );

        var filtered = all.ToList();

        // 🔹 Indicadores agregados
        var summary = new DebtSummaryViewModel
        {
            TotalOrders = filtered.Count,
            TotalAmount = filtered.Sum(x => x.Total - x.Payment),
            MaxSingleDebt = filtered.Max(x => x.Total - x.Payment),
            MaxDaysInDebt = filtered.Where(x => x.DeliveryDate.HasValue).Any()
             ? filtered.Where(x => x.DeliveryDate.HasValue)
                       .Max(x => (today - x.DeliveryDate!.Value.Date).Days)
             : 0,
            AvgDaysInDebt = filtered.Where(x => x.DeliveryDate.HasValue).Any()
             ? (int)filtered.Where(x => x.DeliveryDate.HasValue)
                            .Average(x => (today - x.DeliveryDate!.Value.Date).Days)
             : 0
        };

        // 🔹 Paginación
        var skip = (query.Page - 1) * query.PageSize;
        var pagedEntities = filtered
            .OrderByDescending(x => x.DeliveryDate)
            .Skip(skip)
            .Take(query.PageSize)
            .ToList();

        var viewModels = mapper.Map<List<ViewModel>>(pagedEntities);

        foreach (var vm in viewModels)
        {
            if (vm.DeliveryDate != default)
                vm.DaysInDebt = (today - vm.DeliveryDate.Date).Days;
        }

        return new PagedWithSummaryResult<ViewModel>
        {
            Items = viewModels,
            TotalCount = filtered.Count,
            Page = query.Page,
            PageSize = query.PageSize,
            Summary = summary
        };
    }
}