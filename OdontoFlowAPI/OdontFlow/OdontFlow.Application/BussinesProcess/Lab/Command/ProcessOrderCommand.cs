using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using OrderModel = OdontFlow.Domain.Entities.Order;
using ViewModel = OdontFlow.Domain.ViewModel.StationWork.StationWorkDetailViewModel;
using Model = OdontFlow.Domain.Entities.StationWork;
using UserModel = OdontFlow.Domain.Entities.User;
using OdontFlow.Application.Services.Contracts;

namespace OdontFlow.Application.BussinesProcess.Order.Command;

public class ProcessOrderCommand(Guid id) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id;
}

public class ProcessOrderCommandHandler(
    IWriteOnlyRepository<Guid, Model> writeRepo, 
    IReadOnlyRepository<Guid, Model> readRepo,
    IReadOnlyRepository<Guid, UserModel> readUserRepo,
    IWriteOnlyRepository<Guid, OrderModel> orderRepository, 
    IMapper mapper,
    IUserContext userContext
) : ICommandHandler<ProcessOrderCommand, ViewModel>
{
    public async Task<ViewModel> Handle(ProcessOrderCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var entity = await readRepo.GetAsync(command.Id, new[] { "Order", "Product", "Employee", "WorkStation" })
            ?? throw new NotFoundException("Orden no encontrada.");

        entity.WorkStatus = WORK_STATUS.PROCESO;
        entity.StationStartDate = DateTime.Now;
        entity.EmployeeStartDate = DateTime.Now;
        entity.InProgress = true;

        var user = await readUserRepo.GetAsync(userContext.UserId);
        if (user != null)
        {
            entity.EmployeeId = user.EmployeeId;
        }

        await writeRepo.Modify(entity);

        entity.Order.OrderStatusId = ORDER_STATUS.EN_PROCESO;
        entity.Order.ProcessDate = DateTime.Now;

        await orderRepository.Modify(entity.Order);

        return mapper.Map<ViewModel>(entity);
    }
 
}

