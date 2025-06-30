using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using ViewModel = OdontFlow.Domain.ViewModel.StationWork.StationWorkDetailViewModel;
using Model = OdontFlow.Domain.Entities.StationWork;    
namespace OdontFlow.Application.BussinesProcess.Lab.Command;
public class BlockOrderCommand(Guid id, string message) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id;
    public string Message { get; set; } = message;
}

public class BlockOrderCommandHandler(
    IWriteOnlyRepository<Guid, Model> writeRepo,
    IReadOnlyRepository<Guid, Model> readRepo,
    IMapper mapper
) : ICommandHandler<BlockOrderCommand, ViewModel>
{
    public async Task<ViewModel> Handle(BlockOrderCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var entity = await readRepo.GetAsync(command.Id, new[] { "Order", "WorkStation",  "Product", "Employee" })
            ?? throw new NotFoundException("Orden no encontrada.");

        entity.WorkStatus = WORK_STATUS.BLOQUEADO;
        entity.BlockedDate = DateTime.Now;
        entity.InProgress = false;

        await writeRepo.Modify(entity);
        return mapper.Map<ViewModel>(entity);
    }

}

public class UnBlockOrderCommand(Guid id ) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id; 
}

public class UnBlockOrderCommandHandler(
    IWriteOnlyRepository<Guid, Model> writeRepo,
    IReadOnlyRepository<Guid, Model> readRepo,
    IMapper mapper
) : ICommandHandler<UnBlockOrderCommand, ViewModel>
{
    public async Task<ViewModel> Handle(UnBlockOrderCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var blockedStation =  await readRepo.GetAsync(command.Id, new[] { "Order", "Product", "Employee", "WorkStation" })
            ?? throw new NotFoundException("Estación no encontrada.");

        blockedStation.UnblockedDate = DateTime.Now;
        blockedStation.WorkStatus = WORK_STATUS.ESPERA;
        blockedStation.InProgress = false;

        await writeRepo.Modify(blockedStation);

        var tiempoMuerto = blockedStation.UnblockedDate.Value - blockedStation.BlockedDate.Value;

        // 🚫 No incluir WorkStation para evitar tracking duplicado
        var allStations = await readRepo.GetAllMatchingAsync(
            x => x.OrderId == blockedStation.OrderId
        );

        var estacionesPosteriores = allStations
            .Where(x => x.Step >= blockedStation.Step)
            .OrderBy(x => x.Step)
            .ToList();

        foreach (var station in estacionesPosteriores)
        {
            station.StationStartDate = station.StationStartDate.Add(tiempoMuerto);
            station.StationEndDate = station.StationEndDate.Add(tiempoMuerto);

            if (station.EmployeeStartDate != default)
                station.EmployeeStartDate = station.EmployeeStartDate.Add(tiempoMuerto);

            if (station.EmployeeEndDate != default)
                station.EmployeeEndDate = station.EmployeeEndDate.Add(tiempoMuerto);

            await writeRepo.Modify(station);
        }

        return mapper.Map<ViewModel>(blockedStation);
    }
}





