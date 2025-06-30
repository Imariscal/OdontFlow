using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Product;
using ViewModel = OdontFlow.Domain.ViewModel.Product.ProductViewModel;

namespace OdontFlow.Application.BussinesProcess.Product.Query;

public class GetProductsQuery { }

public class GetProductsQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper)
    : IQueryHandler<GetProductsQuery, IEnumerable<ViewModel>>
{
    public async Task<IEnumerable<ViewModel>> Handle(GetProductsQuery query)
    {
        var entities = await readOnlyRepository.GetAllAsync(false);
        return mapper.Map<IEnumerable<ViewModel>>(entities);
    }
}
