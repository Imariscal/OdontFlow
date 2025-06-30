using ViewModel = OdontFlow.Domain.ViewModel.WorkStation.WorkStationViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.WorkStation.UpdateWorkStationDTO;
using CreateDTO = OdontFlow.Domain.DTOs.WorkStation.CreateWorkStationDTO;
using CreateCommand = OdontFlow.Application.BussinesProcess.WorkStation.Command.CreateWorkStationCommand;
using UpdateCommand = OdontFlow.Application.BussinesProcess.WorkStation.Command.UpdateWorkStationCommand;
using DeleteCommand = OdontFlow.Application.BussinesProcess.WorkStation.Command.DeleteWorkStationCommand;
using GetCommand = OdontFlow.Application.BussinesProcess.WorkStation.Query.GetWorkStationsQuery;
using GetActiveCommand = OdontFlow.Application.BussinesProcess.WorkStation.Query.GetWorkStationsActiveQuery;
using OdontFlow.Application.Services.Contracts;  
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.ViewModel.Client;
using OdontFlow.Domain.DTOs.Client;

namespace OdontFlow.Application.Services;

public class WorkStationService(IMediator mediator) : IWorkStationService
{
    public async Task<ViewModel> CreateAsync(CreateDTO input)
    {
        var handler = mediator.GetCommandHandler<CreateCommand, ViewModel>();
        return await handler.Handle(new CreateCommand(input));
    }

    public Task<ClientViewModel> CreateAsync(CreateClientDTO input)
    {
        throw new NotImplementedException();
    }

    public async Task<ViewModel> DeleteAsync(Guid id)
    {
        var handler = mediator.GetCommandHandler<DeleteCommand, ViewModel>();
        return await handler.Handle(new DeleteCommand(id));
    }

    public async Task<IEnumerable<ViewModel>> GetActiveAsync()
    {
        var handler = mediator.GetQueryHandler<GetActiveCommand, IEnumerable<ViewModel>>();
        return await handler.Handle(new GetActiveCommand());
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
