using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Common;
using OdontFlow.Domain.Repositories.Base;
using OdontFlow.Domain.ViewModel.Order;
using OdontFlow.Domain.ViewModel.Report;
using Model = OdontFlow.Domain.Entities.Order;
using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;

public class GetDebtOrdersExportQuery(OrderAdvancedFilterViewModel input)
{    public OrderAdvancedFilterViewModel Input { get; set; } = input;
}

public class GetDebtOrdersExportQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper
) : IQueryHandler<GetDebtOrdersExportQuery, List<ViewModel>>
{
    public async Task<List<ViewModel>> Handle(GetDebtOrdersExportQuery queryData)
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

        var viewModels = mapper.Map<List<ViewModel>>(all);

        foreach (var vm in viewModels)
        {
            if (vm.DeliveryDate != default)
                vm.DaysInDebt = (today - vm.DeliveryDate.Date).Days;
        }

        return viewModels;
    }
}