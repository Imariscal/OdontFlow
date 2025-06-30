using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Application.BussinesProcess.Order.Command;
using OdontFlow.Domain.Entities;

namespace OdontFlow.Application.Services.Contracts;

public class OrderSequenceService(IMediator mediator) : IOrderSequenceService
{
    public async Task<string> GenerateBarcodeAsync()
    {
        var date = DateTime.Now.ToString("ddMMyy");
        var handler = mediator.GetCommandHandler<CreateOrderSecuenceCommand, OrderSequence>();
        var sequence = await handler.Handle(new CreateOrderSecuenceCommand(date));

        return $"{date}{sequence.LastNumber:D3}";
    }
}
