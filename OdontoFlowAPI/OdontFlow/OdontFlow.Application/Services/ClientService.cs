using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Application.BussinesProcess.PriceList.Command;
using OdontFlow.Application.Services.Contracts;
using ViewModel = OdontFlow.Domain.ViewModel.Client.ClientViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.Client.UpdateClientDTO;
using CreateDTO = OdontFlow.Domain.DTOs.Client.CreateClientDTO;
using OdontFlow.Application.BussinesProcess.Client.Command;
using OdontFlow.Application.BussinesProcess.PriceList.Query;

namespace OdontFlow.Application.Services;

public class ClientService(IMediator mediator) : IClientService
{
    public async Task<ViewModel> CreateAsync(CreateDTO input)
    {
        var handler = mediator.GetCommandHandler<CreateClientCommand, ViewModel>();
        return await handler.Handle(new CreateClientCommand(input));
    }

    public async Task<ViewModel> DeleteAsync(Guid id)
    {
        var handler = mediator.GetCommandHandler<DeleteClientCommand, ViewModel>();
        return await handler.Handle(new DeleteClientCommand(id));
    }

    public async Task<IEnumerable<ViewModel>> GetActiveAsync()
    {
        var handler = mediator.GetQueryHandler<GetClientsActiveQuery, IEnumerable<ViewModel>>();
        return await handler.Handle(new GetClientsActiveQuery());
    }

    public async Task<IEnumerable<ViewModel>> GetAsync()
    {
        var handler = mediator.GetQueryHandler<GetClientsQuery, IEnumerable<ViewModel>>();
        return await handler.Handle(new GetClientsQuery());
    }

    public async Task<ViewModel> UpdateAsync(UpdateDTO input)
    {
        var handler = mediator.GetCommandHandler<UpdateClientCommand, ViewModel>();
        return await handler.Handle(new UpdateClientCommand(input));
    }
}
