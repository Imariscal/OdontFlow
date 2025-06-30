
using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Product;
using ViewModel = OdontFlow.Domain.ViewModel.Product.ProductViewModel;
using PliceListModel = OdontFlow.Domain.Entities.PriceList;
using OdontFlow.Domain.ViewModel.Product;

namespace OdontFlow.Application.BussinesProcess.Product.Query;

public class GetProductsByPlanIdQuery(Guid id)
{
    public Guid Id { get; set; } = id;
}

public class GetProductsByPlanIdQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IReadOnlyRepository<Guid, PliceListModel> readPriceListRepository,
    IMapper mapper)
    : IQueryHandler<GetProductsByPlanIdQuery, IEnumerable<ViewModel>>
{
    public async Task<IEnumerable<ViewModel>> Handle(GetProductsByPlanIdQuery query)
    {
        var priceList = await readPriceListRepository.GetAsync(query.Id, new[] { "Items.Product" });

        if (priceList == null)
            return Enumerable.Empty<ViewModel>();
        var products = priceList.Items
            .Where(i => i.Product != null && i.Active && !i.Deleted)
            .Select(i =>
            {
                var vm = mapper.Map<ProductViewModel>(i.Product);
                vm.Price = i.Price; // Sobrescribimos el precio del producto con el del item
                return vm;
            })
            .OrderBy(p => p.Name)
            .ToList();

        //var entities = await readOnlyRepository.GetAllMatchingAsync(
        //    x => x.ProductCategory == priceList.Category && !x.Deleted && x.Active
        //);

        //var productIds = new HashSet<Guid>(products.Select(p => p.Id));

        //var missingFromEntities = entities
        //    .Where(e => !productIds.Contains(e.Id))
        //    .ToList();

        //var combined = products.Concat(missingFromEntities).ToList();

        return mapper.Map<IEnumerable<ViewModel>>(products);
    }
}
