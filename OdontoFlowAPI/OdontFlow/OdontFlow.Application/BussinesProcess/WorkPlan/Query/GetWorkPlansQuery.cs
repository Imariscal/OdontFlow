
using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base; 
using Model = OdontFlow.Domain.Entities.WorkPlan;
using ViewModel = OdontFlow.Domain.ViewModel.WorkPlan.WorkPlanViewModel;

namespace OdontFlow.Application.BussinesProcess.WorkPlan.Query;

public class GetWorkPlansQuery
{
}

public class GetWorkPlansQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper
) : IQueryHandler<GetWorkPlansQuery, IEnumerable<ViewModel>>
{
    public async Task<IEnumerable<ViewModel>> Handle(GetWorkPlansQuery query)
    {
        var entities = await readOnlyRepository.GetAllAsync(new[]
         {
            "Stations.WorkStation", 
            "Products.Product"
        });

        // Primero, ordenas internamente las estaciones y productos:
        foreach (var entity in entities)
        {
            entity.Stations = entity.Stations.OrderBy(s => s.Order).ToList();
            entity.Products = entity.Products.OrderBy(p => p.Product.Name).ToList();
        }

        entities = entities.OrderBy(o => o.Name);
        // Finalmente, haces el mapeo:
        return mapper.Map<IEnumerable<ViewModel>>(entities);
    }
}