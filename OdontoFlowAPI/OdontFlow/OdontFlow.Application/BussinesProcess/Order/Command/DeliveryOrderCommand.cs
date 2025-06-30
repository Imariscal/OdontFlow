using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Order;
using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;
 

namespace OdontFlow.Application.BussinesProcess.Order.Command;

public class DeliveryOrderCommand(Guid id) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id;
}


public class DeliveryOrderCommandHandler(
    IWriteOnlyRepository<Guid, Model> writeRepo,
    IReadOnlyRepository<Guid, Model> readRepo,
    IMapper mapper
) : ICommandHandler<DeliveryOrderCommand, ViewModel>
{
    public async Task<ViewModel> Handle(DeliveryOrderCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var entity = await readRepo.GetAsync(command.Id, new[] { "Items", "Client", "Client.Employee" })
            ?? throw new NotFoundException("Orden no encontrada.");
        entity.OrderStatusId = ORDER_STATUS.ENTREGADO;
        entity.DeliveryDate = DateTime.Now;
        await writeRepo.Modify(entity); 
        return mapper.Map<ViewModel>(entity);
    } 
}




