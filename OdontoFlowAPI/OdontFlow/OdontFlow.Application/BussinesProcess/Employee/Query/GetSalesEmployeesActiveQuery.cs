using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Employee;
using ViewModel = OdontFlow.Domain.ViewModel.Employee.EmployeeViewModel;

namespace OdontFlow.Application.BussinesProcess.Employee.Query;

public class GetSalesEmployeesActiveQuery
{
}

public class GetSalesEmployeesActiveQueryHandler(
    IReadOnlyRepository<Guid, Model> readRepo,
    IMapper mapper
) : IQueryHandler<GetSalesEmployeesActiveQuery, IEnumerable<ViewModel>>
{
    public async Task<IEnumerable<ViewModel>> Handle(GetSalesEmployeesActiveQuery query)
    {
        var entities = await readRepo.GetAllMatchingAsync(e => e.Active && !e.Deleted && e.IsSalesRepresentative );
        return mapper.Map<IEnumerable<ViewModel>>(entities);
    }
}

