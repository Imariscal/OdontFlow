using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions; 
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.WorkStation;
using ViewModel = OdontFlow.Domain.ViewModel.WorkStation.WorkStationViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.WorkStation.UpdateWorkStationDTO;
using Mapster;

namespace OdontFlow.Application.BussinesProcess.WorkStation.Command;
public class UpdateWorkStationCommand(UpdateDTO input) : ICommand<ViewModel>
{
    public UpdateDTO Input { get; set; } = input;
}

public class UpdateWorkStationCommandHandler(
 IWriteOnlyRepository<Guid, Model> repository,
 IReadOnlyRepository<Guid, Model> readOnlyRepository, 
 //IValidationStrategy<UpdateDTO> validationStrategy,
 IMapper mapper) : ICommandHandler<UpdateWorkStationCommand, ViewModel>
{
    public async Task<ViewModel> Handle(UpdateWorkStationCommand command)
    {
        ArgumentNullException.ThrowIfNull(command.Input);
        ArgumentNullException.ThrowIfNull(command.Input.Id);
         var result = await readOnlyRepository.GetAsync(command.Input.Id) ?? throw new NotFoundException("Client no encontrado");
        command.Input.Adapt(result);
        await repository.Modify(result);
        return mapper.Map<ViewModel>(result);
    }
 
}
