
using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.PriceList;
using ViewModel = OdontFlow.Domain.ViewModel.PriceList.PriceListViewModel;

namespace OdontFlow.Application.BussinesProcess.PriceList.Query;
public class GetPriceListsQuery(bool onlyActive = false)
{
    public bool OnlyActive { get; set; } = onlyActive;
}
public class GetPriceListsQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper)
    : IQueryHandler<GetPriceListsQuery, IEnumerable<ViewModel>>
{
    public async Task<IEnumerable<ViewModel>> Handle(GetPriceListsQuery query)
    {
        var entities = await readOnlyRepository.GetAllAsync(query.OnlyActive);
        return mapper.Map<IEnumerable<ViewModel>>(entities);
    }
}
