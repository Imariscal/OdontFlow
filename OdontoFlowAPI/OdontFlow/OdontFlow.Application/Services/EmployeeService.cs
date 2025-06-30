using ViewModel = OdontFlow.Domain.ViewModel.Employee.EmployeeViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.Employee.CreateEmployeeDTO;
using UpdateDTO = OdontFlow.Domain.DTOs.Employee.UpdateEmployeeDTO;
using CreateCommand = OdontFlow.Application.BussinesProcess.Employee.Command.CreateEmployeeCommand;
using UpdateCommand = OdontFlow.Application.BussinesProcess.Employee.Command.UpdateEmployeeCommand;
using DeleteCommand = OdontFlow.Application.BussinesProcess.Employee.Command.DeleteEmployeeCommand;
using GetCommand = OdontFlow.Application.BussinesProcess.Employee.Query.GetEmployeesQuery;
using GetActiveCommand = OdontFlow.Application.BussinesProcess.Employee.Query.GetEmployeesActiveQuery;
using GetEmplotyeeSalesActiveCommand = OdontFlow.Application.BussinesProcess.Employee.Query.GetSalesEmployeesActiveQuery;
using OdontFlow.Application.Services.Contracts;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Application.BussinesProcess.Employee.Query;

namespace OdontFlow.Application.Services;

public class EmployeeService(IMediator mediator) : IEmployeeService
{
    public async Task<ViewModel> CreateAsync(CreateDTO input)
    {
        var handler = mediator.GetCommandHandler<CreateCommand, ViewModel>();
        return await handler.Handle(new CreateCommand(input));
    }

    public async Task<ViewModel> UpdateAsync(UpdateDTO input)
    {
        var handler = mediator.GetCommandHandler<UpdateCommand, ViewModel>();
        return await handler.Handle(new UpdateCommand(input));
    }

    public async Task<ViewModel> DeleteAsync(Guid id)
    {
        var handler = mediator.GetCommandHandler<DeleteCommand, ViewModel>();
        return await handler.Handle(new DeleteCommand(id));
    }

    public async Task<IEnumerable<ViewModel>> GetAsync()
    {
        var handler = mediator.GetQueryHandler<GetCommand, IEnumerable<ViewModel>>();
        return await handler.Handle(new GetCommand());
    }

    public async Task<IEnumerable<ViewModel>> GetActiveAsync()
    {
        var handler = mediator.GetQueryHandler<GetActiveCommand, IEnumerable<ViewModel>>();
        return await handler.Handle(new GetActiveCommand());
    }

    public async Task<IEnumerable<ViewModel>> GetSalesEmployeeActiveAsync()
    {
        var handler = mediator.GetQueryHandler<GetEmplotyeeSalesActiveCommand, IEnumerable<ViewModel>>();
        return await handler.Handle(new GetEmplotyeeSalesActiveCommand());
    }
}
