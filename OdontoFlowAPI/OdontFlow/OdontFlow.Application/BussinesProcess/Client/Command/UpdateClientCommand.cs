using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions; 
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Client;
using ModelEmployee = OdontFlow.Domain.Entities.Employee;
using ViewModel = OdontFlow.Domain.ViewModel.Client.ClientViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.Client.UpdateClientDTO;
using Mapster;

namespace OdontFlow.Application.BussinesProcess.Client.Command;
public class UpdateClientCommand(UpdateDTO input) : ICommand<ViewModel>
{
    public UpdateDTO Input { get; set; } = input;
}

public class UpdateClientCommandHandler(
 IWriteOnlyRepository<Guid, Model> repository,
 IReadOnlyRepository<Guid, Model> readOnlyRepository,
  IReadOnlyRepository<Guid, ModelEmployee> employeeReadOnlyRepository,
 //IValidationStrategy<UpdateDTO> validationStrategy,
 IMapper mapper) : ICommandHandler<UpdateClientCommand, ViewModel>
{
    public async Task<ViewModel> Handle(UpdateClientCommand command)
    {
        ArgumentNullException.ThrowIfNull(command.Input);
        ArgumentNullException.ThrowIfNull(command.Input.Id);
       // Validate(command.Input);
        var result = await readOnlyRepository.GetAsync(command.Input.Id) ?? throw new NotFoundException("Client no encontrado");
        command.Input.Adapt(result);


        if (result.ClientInvoice != null)
        {
            result.ClientInvoice.Active = true;
            result.ClientInvoice.Deleted = false;
            result.ClientInvoice.CreationDate = DateTime.UtcNow;
            result.ClientInvoice.CreatedBy = "Super visor";

            result.ClientInvoice.LastModificationDate = DateTime.UtcNow;
            result.ClientInvoice.LastModifiedBy = "Super visor";
        }
        result.Employee = await employeeReadOnlyRepository.GetAsync(result.EmployeeId);
        await repository.Modify(result);
        return mapper.Map<ViewModel>(result);
    }

    //private void Validate(UpdateDTO entity)
    //{
    //    var validationResult = validationStrategy.Validate(entity);
    //    if (!validationResult.IsValid) throw new BusinessValidationException(validationResult.Errors);
    //}
}
