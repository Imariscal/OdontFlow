using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using OdontFlow.Domain.ViewModel.Supplier;
using Model = OdontFlow.Domain.Entities.Supplier;

namespace OdontFlow.Application.BussinesProcess.Supplier.Query;

public class GetSuppliersQuery { }

public class GetSuppliersQueryHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper)
    : IQueryHandler<GetSuppliersQuery, IEnumerable<SupplierViewModel>>
{
    public async Task<IEnumerable<SupplierViewModel>> Handle(GetSuppliersQuery query)
    {
        var entities = await readOnlyRepository.GetAllAsync(false);
        return mapper.Map<IEnumerable<SupplierViewModel>>(entities);
    }
}
