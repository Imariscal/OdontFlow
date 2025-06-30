
using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.PriceListItem;
using ViewModel = OdontFlow.Domain.ViewModel.PriceList.PriceListItemViewModel;

namespace OdontFlow.Application.BussinesProcess.PriceList.Query;
public class GetPriceListItemsQuery(Guid id)
{
    public Guid Id { get; set; } = id;
}
public class GetPriceListItemsQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper)
    : IQueryHandler<GetPriceListItemsQuery, IEnumerable<ViewModel>>
{
    public async Task<IEnumerable<ViewModel>> Handle(GetPriceListItemsQuery query)
    {
        var entities = await readOnlyRepository.GetAllMatchingAsync(c => c.PriceListId == query.Id && !c.Deleted, new[] { "Product" } );
        return mapper.Map<IEnumerable<ViewModel>>(entities);
    }
}
