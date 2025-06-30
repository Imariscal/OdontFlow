using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.WorkStation;
using ViewModel = OdontFlow.Domain.ViewModel.WorkStation.WorkStationViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.WorkStation.CreateWorkStationDTO;

namespace OdontFlow.Application.BussinesProcess.WorkStation.Command;
public class CreateWorkStationCommand(CreateDTO input) : ICommand<ViewModel>
{
    public CreateDTO Input { get; set; } = input;
}
public class CreateWorkStationCommandHandler(
     IWriteOnlyRepository<Guid, Model> writeOnlyRepository,
     IReadOnlyRepository<Guid, Model> readOnlyRepository,
     //IValidationStrategy<CreateDTO> validationStrategy,
     IMapper mapper)
      : ICommandHandler<CreateWorkStationCommand, ViewModel>
{
    public async Task<ViewModel> Handle(CreateWorkStationCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var entityExist = await readOnlyRepository.GetAllMatchingAsync(x => x.Name == command.Input.Name);

        if (entityExist.Any())
        {
            throw new InvalidOperationException("Ya existe una estacion con el mismo nombre.");
        }

        var entity = mapper.Map<Model>(command.Input); 
        await writeOnlyRepository.AddAsync(entity);
        return mapper.Map<ViewModel>(entity);
    } 
}
