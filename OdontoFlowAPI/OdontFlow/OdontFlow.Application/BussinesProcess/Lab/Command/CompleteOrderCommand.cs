using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using OrderModel = OdontFlow.Domain.Entities.Order;
using ViewModel = OdontFlow.Domain.ViewModel.StationWork.StationWorkDetailViewModel;
using Model = OdontFlow.Domain.Entities.StationWork;    
namespace OdontFlow.Application.BussinesProcess.Lab.Command;
public class CompleteOrderCommand(Guid id) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id;
}

public class CompleteOrderCommandHandler(
    IWriteOnlyRepository<Guid, Model> writeRepo,
    IReadOnlyRepository<Guid, Model> readRepo,
    IWriteOnlyRepository<Guid, OrderModel> orderRepository,
    IMapper mapper
) : ICommandHandler<CompleteOrderCommand, ViewModel>
{
    public async Task<ViewModel> Handle(CompleteOrderCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var entity = await readRepo.GetAsync(command.Id, new[] { "Order", "Product", "Employee", "WorkStation" })
            ?? throw new NotFoundException("Orden no encontrada.");

        entity.WorkStatus = WORK_STATUS.TERMINADO;
        entity.EmployeeEndDate = DateTime.Now;
        entity.InProgress = false;

        await writeRepo.Modify(entity);

        // Obtener todas las estaciones de la orden
        var allSteps = await readRepo.GetAllMatchingAsync(o => o.OrderId == entity.OrderId);

        // Si todas las estaciones están terminadas, marcar la orden como TERMINADA
        var allCompleted = allSteps.All(x => x.WorkStatus == WORK_STATUS.TERMINADO);

        if (allCompleted)
        {
            entity.Order.OrderStatusId = ORDER_STATUS.TERMINADO;
            entity.Order.CompleteDate = DateTime.Now;
            await orderRepository.Modify(entity.Order);
        }

        return mapper.Map<ViewModel>(entity);
    }

}


