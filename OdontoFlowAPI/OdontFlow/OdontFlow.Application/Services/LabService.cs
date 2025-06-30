using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Application.BussinesProcess.Lab.Command;
using OdontFlow.Application.BussinesProcess.Lab.Query;
using OdontFlow.Application.BussinesProcess.Order.Command;
using OdontFlow.Application.BussinesProcess.StationWork.Query;
using OdontFlow.Application.Services.Contracts;
using OdontFlow.Domain.ViewModel.StationWork;
using ViewModel = OdontFlow.Domain.ViewModel.Lab.StationWorkSummaryViewModel;

namespace OdontFlow.Application.Services;

public class LabService(IMediator mediator) : ILabService
{
 
    public async Task<StationWorkDetailViewModel> CompleteWorkStationDetail(Guid Id)
    {
        var handler = mediator.GetCommandHandler<CompleteOrderCommand, StationWorkDetailViewModel>();
        return await handler.Handle(new CompleteOrderCommand(Id));
    }

    public async Task<ViewModel> GetAsync()
    {
        var handler = mediator.GetQueryHandler<GetStationWorkSummaryQuery, ViewModel>();
        return await handler.Handle(new GetStationWorkSummaryQuery());
    }

    public async Task<IEnumerable<StationWorkDetailViewModel>> GetWorkStationDetail(Guid Id)
    {
        var handler = mediator.GetQueryHandler<GetStationWorkDetailsQuery, IEnumerable<StationWorkDetailViewModel>>();
        return await handler.Handle(new GetStationWorkDetailsQuery(Id));
    }

    public async Task<StationWorkDetailViewModel> ProcessWorkStationDetail(Guid Id)
    {
        var handler = mediator.GetCommandHandler<ProcessOrderCommand, StationWorkDetailViewModel>();
        return await handler.Handle(new ProcessOrderCommand(Id));
    }

    public async Task<StationWorkDetailViewModel> RejectWorkStationDetail(Guid Id, string message)
    {
        var handler = mediator.GetCommandHandler<RejectOrderCommand, StationWorkDetailViewModel>();
        return await handler.Handle(new RejectOrderCommand(Id, message));
    }

    public async Task<StationWorkDetailViewModel> UnBlockWorkStationDetail(Guid Id)
    {
        var handler = mediator.GetCommandHandler<UnBlockOrderCommand, StationWorkDetailViewModel>();
        return await handler.Handle(new UnBlockOrderCommand(Id));
    }

    public async Task<StationWorkDetailViewModel> BlockWorkStationDetail(Guid Id, string message)
    {
        var handler = mediator.GetCommandHandler<BlockOrderCommand, StationWorkDetailViewModel>();
        return await handler.Handle(new BlockOrderCommand(Id, message));
    }

}
