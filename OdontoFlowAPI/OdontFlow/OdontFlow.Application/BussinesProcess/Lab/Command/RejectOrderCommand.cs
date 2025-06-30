using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using ViewModel = OdontFlow.Domain.ViewModel.StationWork.StationWorkDetailViewModel;
using Model = OdontFlow.Domain.Entities.StationWork;


namespace OdontFlow.Application.BussinesProcess.Lab.Command;

public class RejectOrderCommand(Guid id, string message) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id;
    public string Message { get; set; } = message;
}

public class RejectOrderCommandHandler(
    IWriteOnlyRepository<Guid, Model> writeRepo,
    IReadOnlyRepository<Guid, Model> readRepo, 
    IMapper mapper
) : ICommandHandler<RejectOrderCommand, ViewModel>
{
    public async Task<ViewModel> Handle(RejectOrderCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var entity = await readRepo.GetAsync(command.Id, new[] { "Order", "Product" })
            ?? throw new NotFoundException("Orden no encontrada.");

        entity.WorkStatus = WORK_STATUS.ESPERA; 
        entity.InProgress = false;
        await writeRepo.Modify(entity);

        var stepsResult = await readRepo.GetAllMatchingAsync(o => o.OrderId == entity.OrderId && o.Step == entity.Step - 1); 
        var step = stepsResult.FirstOrDefault();

        if (step.Step == entity.Step - 1)
        {
            step.WorkStatus = WORK_STATUS.RECHAZADO;
            await writeRepo.Modify(step);
        }

        return mapper.Map<ViewModel>(entity);
    }

}


