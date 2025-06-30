

using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base; 
using Model = OdontFlow.Domain.Entities.Product;
using ViewModel = OdontFlow.Domain.ViewModel.Product.ProductViewModel;
namespace OdontFlow.Application.BussinesProcess.Product.Query;
public class GetProductsWithoutPlanQuery { }

public class GetProductsWithoutPlanQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper)
    : IQueryHandler<GetProductsWithoutPlanQuery, IEnumerable<ViewModel>>
{
    public async Task<IEnumerable<ViewModel>> Handle(GetProductsWithoutPlanQuery query)
    {
        var entities = await readOnlyRepository.GetAllMatchingAsync(
              p => !p.WorkPlans.Any() && p.Active && !p.Deleted
         );
        return mapper.Map<IEnumerable<ViewModel>>(entities);
    }
}
