using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Employee;
using ViewModel = OdontFlow.Domain.ViewModel.Employee.EmployeeViewModel;

namespace OdontFlow.Application.BussinesProcess.Employee.Query;
public class GetEmployeesActiveQuery { }

public class GetEmployeesActiveQueryHandler(
    IReadOnlyRepository<Guid, Model> readRepo,
    IMapper mapper
) : IQueryHandler<GetEmployeesActiveQuery, IEnumerable<ViewModel>>
{
    public async Task<IEnumerable<ViewModel>> Handle(GetEmployeesActiveQuery query)
    {
        var entities = await readRepo.GetAllMatchingAsync(e => e.Active && !e.Deleted );
        return mapper.Map<IEnumerable<ViewModel>>(entities);
    }
}
