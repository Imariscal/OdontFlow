using Model = OdontFlow.Domain.Entities.WorkPlan;
using ViewModel = OdontFlow.Domain.ViewModel.WorkPlan.WorkPlanViewModel; 
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using MapsterMapper;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
namespace OdontFlow.Application.BussinesProcess.WorkPlan.Command;

public class DeleteWorkPlanCommand(Guid id) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id;
}

public class DeleteWorkPlanCommandHandler(
    IWriteOnlyRepository<Guid, Model> repository,
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper
) : ICommandHandler<DeleteWorkPlanCommand, ViewModel>
{
    public async Task<ViewModel> Handle(DeleteWorkPlanCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var entity = await readOnlyRepository.GetAsync(command.Id)
            ?? throw new NotFoundException("Plan de trabajo NO encontrado.");

        // TODO: Validar si existen relaciones activas antes de eliminar, como planes usados en citas u órdenes.

        await repository.Remove(entity);

        return mapper.Map<ViewModel>(entity);
    }
}