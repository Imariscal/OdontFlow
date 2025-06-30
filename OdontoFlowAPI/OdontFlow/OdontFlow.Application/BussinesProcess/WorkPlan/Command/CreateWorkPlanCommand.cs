using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.WorkPlan;
using ViewModel = OdontFlow.Domain.ViewModel.WorkPlan.WorkPlanViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.WorksPlan.CreateWorkPlanDTO;
using OdontFlow.Domain.Entities; 

namespace OdontFlow.Application.BussinesProcess.WorkPlan.Command;

public class CreateWorkPlanCommand(CreateDTO input) : ICommand<ViewModel>
{
    public CreateDTO Input { get; set; } = input;
}
public class CreateWorkPlanCommandHandler(
    IWriteOnlyRepository<Guid, Model> writeOnlyRepository,
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IMapper mapper
) : ICommandHandler<CreateWorkPlanCommand, ViewModel>
{
    public async Task<ViewModel> Handle(CreateWorkPlanCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var exists = await readOnlyRepository.GetAllMatchingAsync(x => x.Name == command.Input.Name);
        if (exists.Any())
            throw new InvalidOperationException("Ya existe un plan de trabajo con el mismo nombre.");

        var entity = mapper.Map<Domain.Entities.WorkPlan>(command.Input);

        // Relación estaciones
        entity.Stations = command.Input.Stations
            .Select(x => new WorkStationPlan
            {
                Id =Guid.NewGuid(),
                WorkStationId = x.WorkStationId,
                Order = x.Order,
                PlanId = entity.Id,
                Active = true,
                Deleted = false,
                CreationDate = DateTime.UtcNow,
                CreatedBy = "Super visor",
                LastModificationDate = DateTime.UtcNow,
                LastModifiedBy = "Super visor"
            }).ToList();

        // Relación productos
        entity.Products = command.Input.ProductIds
            .Select(pid => new WorkPlanProducts
            {
                Id = Guid.NewGuid(),
                ProductId = pid,
                PlanId = entity.Id,
                Active = true,
                Deleted = false,
                CreationDate = DateTime.UtcNow,
                CreatedBy = "Super visor",

                LastModificationDate = DateTime.UtcNow,
                LastModifiedBy = "Super visor"
            }).ToList();

        await writeOnlyRepository.AddAsync(entity);

        return mapper.Map<ViewModel>(entity);
    }
}

