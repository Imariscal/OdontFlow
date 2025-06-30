using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Client;
using ViewModel = OdontFlow.Domain.ViewModel.Client.ClientViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.Client.CreateClientDTO;
using ModelEmployee = OdontFlow.Domain.Entities.Employee;

namespace OdontFlow.Application.BussinesProcess.Client.Command;

public class CreateClientCommand(CreateDTO input) : ICommand<ViewModel>
{
    public CreateDTO Input { get; set; } = input;
}

public class CreateClientCommandHandler(
     IWriteOnlyRepository<Guid, Model> writeOnlyRepository,
     IReadOnlyRepository<Guid, Model> readOnlyRepository,
       IReadOnlyRepository<Guid, ModelEmployee> employeeReadOnlyRepository,
 
     IMapper mapper)
      : ICommandHandler<CreateClientCommand, ViewModel>
{
    public async Task<ViewModel> Handle(CreateClientCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        //Validate(command.Input);

        var entityExist = await readOnlyRepository.GetAllMatchingAsync(x => x.Name == command.Input.Name);

        if (entityExist.Any())
        {
            throw new InvalidOperationException("Ya existe en la base de datos un cliente con el mismo nombre.");
        }

        var entity = mapper.Map<Model>(command.Input);

        if (entity.ClientInvoice != null)
        {
            entity.ClientInvoice.Active = true;
            entity.ClientInvoice.Deleted = false;
            entity.ClientInvoice.CreationDate = DateTime.UtcNow;
            entity.ClientInvoice.CreatedBy = "Super visor";

            entity.ClientInvoice.LastModificationDate = DateTime.UtcNow;
            entity.ClientInvoice.LastModifiedBy = "Super visor";
        }
        await writeOnlyRepository.AddAsync(entity);
        entity.Employee = await employeeReadOnlyRepository.GetAsync(entity.EmployeeId);
        return mapper.Map<ViewModel>(entity);
    }

    //private void Validate(CreateDTO entityDTO)
    //{
    //    var validationResult = validationStrategy.Validate(entityDTO);
    //    if (!validationResult.IsValid) throw new BusinessValidationException(validationResult.Errors);
    //}
}
