using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base; 
using Model = OdontFlow.Domain.Entities.Product;
using ViewModel = OdontFlow.Domain.ViewModel.Product.ProductViewModel;

namespace OdontFlow.Application.BussinesProcess.Product.Query;

public class GetProductsByCategoryQuery(PRODUCT_CATEGORY id)
{
    public PRODUCT_CATEGORY category { get; set; } = id;
}

public class GetProductsByCategoryQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper)
    : IQueryHandler<GetProductsByCategoryQuery, IEnumerable<ViewModel>>
{
    public async Task<IEnumerable<ViewModel>> Handle(GetProductsByCategoryQuery query)
    {
        var entities = await readOnlyRepository.GetAllMatchingAsync(x => x.ProductCategory == query.category && !x.Deleted && x.Active);
        return mapper.Map<IEnumerable<ViewModel>>(entities);
    }
}
