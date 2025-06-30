using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Common;
using OdontFlow.Domain.Repositories.Base;
using OdontFlow.Domain.ViewModel.Order;
using Model = OdontFlow.Domain.Entities.Order;
using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;

namespace OdontFlow.Application.BussinesProcess.Reports.Query;

public class GetDebtOrdersQuery(OrderAdvancedFilterViewModel input)
{
    public OrderAdvancedFilterViewModel Input { get; set; } = input;
}

public class GetDebtOrdersQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper
) : IQueryHandler<GetDebtOrdersQuery, PagedResult<ViewModel>>
{
    public async Task<PagedResult<ViewModel>> Handle(GetDebtOrdersQuery queryData)
    {
        var query = queryData.Input;
        var today = DateTime.Now.Date;

        // 🔹 1. Obtener todas las órdenes con deuda real y entregadas
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

        // 🔹 2. Paginación de entidades
        var skip = (query.Page - 1) * query.PageSize;
        var pagedEntities = all
            .OrderByDescending(x => x.DeliveryDate)
            .Skip(skip)
            .Take(query.PageSize)
            .ToList();

        // 🔹 3. Mapear con Mapster
        var viewModels = mapper.Map<List<ViewModel>>(pagedEntities);

        // 🔹 4. Calcular DaysInDebt después del mapeo
        foreach (var vm in viewModels)
        {
            if (vm.DeliveryDate != default)
                vm.DaysInDebt = (today - vm.DeliveryDate.Date).Days;
        }

        return new PagedResult<ViewModel>(
            viewModels,
            all.Count(),
            query.Page,
            query.PageSize
        );
    }

}
