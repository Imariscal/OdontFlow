using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.User;
using ViewModel = OdontFlow.Domain.ViewModel.User.UserViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.User.UpdateUserDTO;
using Mapster; 

namespace OdontFlow.Application.BussinesProcess.Auth.Command;

public class UpdateRegisterCommand(UpdateDTO input) : ICommand<ViewModel>
{
    public UpdateDTO Input { get; set; } = input;
}


public class UpdateRegisterCommandHandler(
 IWriteOnlyRepository<Guid, Model> repository,
 IReadOnlyRepository<Guid, Model> readOnlyRepository,
 IMapper mapper) : ICommandHandler<UpdateRegisterCommand, ViewModel>
{
    public async Task<ViewModel> Handle(UpdateRegisterCommand command)
    {
        ArgumentNullException.ThrowIfNull(command.Input);
        ArgumentNullException.ThrowIfNull(command.Input.Id);
 
        var result = await readOnlyRepository.GetAsync(command.Input.Id) ?? throw new NotFoundException("Usuario no encontrado");
        command.Input.Adapt(result);
        await repository.Modify(result);
        return mapper.Map<ViewModel>(result);
    }
}

