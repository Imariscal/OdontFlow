using ViewModel = OdontFlow.Domain.ViewModel.OrderPayment.OrderPaymentViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.OrderPayment.CreateOrderPaymentDTO;
using UpdateDTO = OdontFlow.Domain.DTOs.OrderPayment.UpdateOrderPaymentDTO;
using CreateCommand = OdontFlow.Application.BussinesProcess.OrderPayment.Command.CreateOrderPaymentCommand;
using UpdateCommand = OdontFlow.Application.BussinesProcess.OrderPayment.Command.UpdateOrderPaymentCommand;
using DeleteCommand = OdontFlow.Application.BussinesProcess.OrderPayment.Command.DeleteOrderPaymentCommand;
using GetQuery = OdontFlow.Application.BussinesProcess.OrderPayment.Query.GetOrderPaymentsQuery;

using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Application.Services.Contracts;
using OdontFlow.CrossCutting.Common;


namespace OdontFlow.Application.Services;

public class OrderPaymentService(IMediator mediator) : IOrderPaymentService
{
    public async Task<IEnumerable<ViewModel>> GetAsync(Guid id)
    {
        var handler = mediator.GetQueryHandler<GetQuery, IEnumerable<ViewModel>>();
        return await handler.Handle(new GetQuery(id));
    } 

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
}