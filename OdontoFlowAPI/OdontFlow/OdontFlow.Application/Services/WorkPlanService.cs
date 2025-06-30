using ViewModel = OdontFlow.Domain.ViewModel.WorkPlan.WorkPlanViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.WorksPlan.UpdateWorkPlanDTO;
using CreateDTO = OdontFlow.Domain.DTOs.WorksPlan.CreateWorkPlanDTO;
using CreateCommand = OdontFlow.Application.BussinesProcess.WorkPlan.Command.CreateWorkPlanCommand;
using UpdateCommand = OdontFlow.Application.BussinesProcess.WorkPlan.Command.UpdateWorkPlanCommand;
using DeleteCommand = OdontFlow.Application.BussinesProcess.WorkPlan.Command.DeleteWorkPlanCommand;
using GetCommand = OdontFlow.Application.BussinesProcess.WorkPlan.Query.GetWorkPlansQuery;
using OdontFlow.Application.Services.Contracts;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.ViewModel.Client;
using OdontFlow.Domain.DTOs.Client;

namespace OdontFlow.Application.Services;

public class WorkPlanService(IMediator mediator) : IWorkPlanService
{
    public async Task<ViewModel> CreateAsync(CreateDTO input)
    {
        var handler = mediator.GetCommandHandler<CreateCommand, ViewModel>();
        return await handler.Handle(new CreateCommand(input));
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

    public async Task<ViewModel> UpdateAsync(UpdateDTO input)
    {
        var handler = mediator.GetCommandHandler<UpdateCommand, ViewModel>();
        return await handler.Handle(new UpdateCommand(input));
    }

}