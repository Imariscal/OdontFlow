using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.OrderItem;
using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;
using StationWorkModel = OdontFlow.Domain.Entities.StationWork;
namespace OdontFlow.Application.BussinesProcess.Order.Command;

public class DeleteOrderItemCommand(Guid id) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id;
}
public class DeleteOrderItemCommandHandler(
    IWriteOnlyRepository<Guid, Model> repository,
    IReadOnlyRepository<Guid, Model> readRepository,
    IReadOnlyRepository<Guid, StationWorkModel> readWorkStationRepository,
    IWriteOnlyRepository<Guid, StationWorkModel> writeWorkStationRepository,
    IMapper mapper
) : ICommandHandler<DeleteOrderItemCommand, ViewModel>
{
    public async Task<ViewModel> Handle(DeleteOrderItemCommand command)
    {
        var entity = await readRepository.GetAsync(command.Id)
            ?? throw new NotFoundException("Orden Item no encontrada.");
        // Validar si alguno de los StationWork relacionados ya está en proceso o terminado
        var relatedWorks = await readWorkStationRepository.GetAllMatchingAsync(
            o => o.ProductId == entity.ProductId && o.OrderId == entity.OrderId);

        if (relatedWorks.Any(sw => sw.WorkStatus != WORK_STATUS.ESPERA))
        {
            throw new NotFoundException("El producto ya inició producción y no puede eliminarse.");
        }

        // Marcar como eliminado los StationWorks en espera
        foreach (var sw in relatedWorks)
        {
            sw.Deleted = true;
            await writeWorkStationRepository.Modify(sw);
        }

        // Marcar el OrderItem como eliminado (soft delete)
        entity.Deleted = true;
        await repository.Modify(entity);

        return mapper.Map<ViewModel>(entity);
    }
}

