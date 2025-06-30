using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.BusinessRules.Base;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Supplier;
using ViewModel = OdontFlow.Domain.ViewModel.Supplier.SupplierViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.Supplier.UpdateSupplierDTO;
using Mapster;

namespace OdontFlow.Application.BussinesProcess.Supplier.Command;

public class UpdateSupplierCommand(UpdateDTO input) : ICommand<ViewModel>
{
    public UpdateDTO Input { get; set; } = input;
}

public class UpdateSupplierCommandHandler(
 IWriteOnlyRepository<Guid, Model> repository,
 IReadOnlyRepository<Guid, Model> readOnlyRepository, IMapper mapper,
 IValidationStrategy<UpdateDTO> validationStrategy) : ICommandHandler<UpdateSupplierCommand, ViewModel>
{
    public async Task<ViewModel> Handle(UpdateSupplierCommand command)
    {
        ArgumentNullException.ThrowIfNull(command.Input);
        ArgumentNullException.ThrowIfNull(command.Input.Id);
        Validate(command.Input);
        var result = await readOnlyRepository.GetAsync(command.Input.Id) ?? throw new NotFoundException("Proveedor no encontrado");
        command.Input.Adapt(result);
        await repository.Modify(result);
        return mapper.Map<ViewModel>(result);
    }

    private void Validate(UpdateDTO entity)
    {
        var validationResult = validationStrategy.Validate(entity);
        if (!validationResult.IsValid) throw new BusinessValidationException(validationResult.Errors);
    }
}

