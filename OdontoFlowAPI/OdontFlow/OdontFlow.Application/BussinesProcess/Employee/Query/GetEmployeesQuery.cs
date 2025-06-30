using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Employee;
using ViewModel = OdontFlow.Domain.ViewModel.Employee.EmployeeViewModel;

namespace OdontFlow.Application.BussinesProcess.Employee.Query;
public class GetEmployeesQuery { }

public class GetEmployeesQueryHandler(
    IReadOnlyRepository<Guid, Model> readRepo,
    IMapper mapper
) : IQueryHandler<GetEmployeesQuery, IEnumerable<ViewModel>>
{
    public async Task<IEnumerable<ViewModel>> Handle(GetEmployeesQuery query)
    {
        var entities = await readRepo.GetAllAsync();
        return mapper.Map<IEnumerable<ViewModel>>(entities);
    }
}
