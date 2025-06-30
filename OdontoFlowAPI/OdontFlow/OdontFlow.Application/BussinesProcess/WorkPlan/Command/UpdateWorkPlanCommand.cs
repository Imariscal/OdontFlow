using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using Model = OdontFlow.Domain.Entities.WorkPlan;
using ViewModel = OdontFlow.Domain.ViewModel.WorkPlan.WorkPlanViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.WorksPlan.UpdateWorkPlanDTO;
using OdontFlow.Domain.Entities;
using Mapster;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;

namespace OdontFlow.Application.BussinesProcess.WorkPlan.Command;

public class UpdateWorkPlanCommand(UpdateDTO input) : ICommand<ViewModel>
{
    public UpdateDTO Input { get; set; } = input;
}

public class UpdateWorkPlanCommandHandler(
    IWriteOnlyRepository<Guid, Model> writeRepo,
    IWriteOnlyRepository<Guid, WorkStationPlan> writeWorkStationPlan,
    IWriteOnlyRepository<Guid, WorkPlanProducts> writeWorkPlanProducts,
    IReadOnlyRepository<Guid, Model> readRepo,
    IMapper mapper
) : ICommandHandler<UpdateWorkPlanCommand, ViewModel>
{
    public async Task<ViewModel> Handle(UpdateWorkPlanCommand command)
    {
        ArgumentNullException.ThrowIfNull(command.Input);
        ArgumentNullException.ThrowIfNull(command.Input.Id);

        var entity = await readRepo.GetAsync(command.Input.Id, new[] { "Stations", "Products" })
            ?? throw new NotFoundException("Plan de trabajo no encontrado.");

        // 2. Actualizar datos simples 
        entity.LastModificationDate = DateTime.UtcNow;
        entity.LastModifiedBy = "Supervisor";
        entity.Active = command.Input.Active;

        // 3. Eliminar relaciones anteriores
        await writeWorkStationPlan.RemoveRange(entity.Stations.ToList(), applyPhysical: true);
        await writeWorkPlanProducts.RemoveRange(entity.Products.ToList(), applyPhysical: true);

        // 4. Guardar entidad padre (WorkPlan) sola
        await writeRepo.Modify(entity);

        // 5. Insertar nuevas estaciones
        var newStations = command.Input.Stations.Select((station, index) => new WorkStationPlan
        {
            Id = Guid.NewGuid(),
            PlanId = entity.Id,
            WorkStationId = station.WorkStationId,
            Order = station.Order,
            Active = true,
            Deleted = false,
            CreationDate = DateTime.UtcNow,
            CreatedBy = "Supervisor",
            LastModificationDate = DateTime.UtcNow,
            LastModifiedBy = "Supervisor"
        }).ToList();

        await writeWorkStationPlan.AddRangeAsync(newStations);

        // 6. Insertar nuevos productos
        var newProducts = command.Input.ProductIds.Select(productId => new WorkPlanProducts
        {
            Id = Guid.NewGuid(),
            PlanId = entity.Id,
            ProductId = productId,
            Active = true,
            Deleted = false,
            CreationDate = DateTime.UtcNow,
            CreatedBy = "Supervisor",
            LastModificationDate = DateTime.UtcNow,
            LastModifiedBy = "Supervisor"
        }).ToList();

        await writeWorkPlanProducts.AddRangeAsync(newProducts);

        return mapper.Map<ViewModel>(entity);
    }
}