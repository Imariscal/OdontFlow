using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.BusinessRules.Base; 
using OdontFlow.Domain.Repositories.Base; 
using Model = OdontFlow.Domain.Entities.Supplier;
using ViewModel = OdontFlow.Domain.ViewModel.Supplier.SupplierViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.Supplier.CreateSupplierDTO;

namespace OdontFlow.Application.BussinesProcess.Supplier.Command;
public class CreateSupplierCommand(CreateDTO input) : ICommand<ViewModel>
{
    public CreateDTO Input { get; set; } = input;
}

public class CreateSupplierCommandHandler(
     IWriteOnlyRepository<Guid, Model> writeOnlyRepository,
     IValidationStrategy<CreateDTO> validationStrategy, IMapper mapper)
      : ICommandHandler<CreateSupplierCommand, ViewModel>
{
    public async Task<ViewModel> Handle(CreateSupplierCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        Validate(command.Input);
        var entity = mapper.Map<Model>(command.Input);
        await writeOnlyRepository.AddAsync(entity);
        return mapper.Map<ViewModel>(entity);
    }

    private void Validate(CreateDTO entityDTO)
    {
        var validationResult = validationStrategy.Validate(entityDTO);
        if (!validationResult.IsValid) throw new BusinessValidationException(validationResult.Errors);
    }
}
