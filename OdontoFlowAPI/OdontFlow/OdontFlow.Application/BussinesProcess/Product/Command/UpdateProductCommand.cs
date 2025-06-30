using Mapster;
using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.BusinessRules.Base;
using OdontFlow.Domain.DTOs.Product;
using OdontFlow.Domain.Repositories.Base;
using OdontFlow.Domain.ViewModel.Product;
using Model = OdontFlow.Domain.Entities.Product;

namespace OdontFlow.Application.BussinesProcess.Product.Command;

public class UpdateProductCommand(UpdateProductDTO input) : ICommand<ProductViewModel>
{
    public UpdateProductDTO Input { get; set; } = input;
}

public class UpdateProductCommandHandler(
 IWriteOnlyRepository<Guid, Model> repository,
 IReadOnlyRepository<Guid, Model> readOnlyRepository, IMapper mapper,
 IValidationStrategy<UpdateProductDTO> validationStrategy) : ICommandHandler<UpdateProductCommand, ProductViewModel>
{
    public async Task<ProductViewModel> Handle(UpdateProductCommand command)
    {
        ArgumentNullException.ThrowIfNull(command.Input);
        ArgumentNullException.ThrowIfNull(command.Input.Id);

        Validate(command.Input);

        var result = await readOnlyRepository.GetAsync(command.Input.Id) ?? throw new NotFoundException("Producto No encontrado.");
        command.Input.Adapt(result);

        await repository.Modify(result);

        return mapper.Map<ProductViewModel>(result);
    }

    private void Validate(UpdateProductDTO entity)
    {
        var validationResult = validationStrategy.Validate(entity);
        if (!validationResult.IsValid) throw new BusinessValidationException(validationResult.Errors);
    }
}

