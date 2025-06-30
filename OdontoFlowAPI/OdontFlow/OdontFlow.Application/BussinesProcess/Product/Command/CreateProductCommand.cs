using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.BusinessRules.Base;
using OdontFlow.Domain.DTOs.Product;
using OdontFlow.Domain.Repositories.Base;
using OdontFlow.Domain.ViewModel.Product;
using Model = OdontFlow.Domain.Entities.Product;

namespace OdontFlow.Application.BussinesProcess.Product.Command;

public class CreateProductCommand(CreateProductDTO input) : ICommand<ProductViewModel>
{
    public CreateProductDTO Input { get; set; } = input;
}

public class CreateProductCommandHandler(
     IWriteOnlyRepository<Guid, Model> writeOnlyRepository,
     IReadOnlyRepository<Guid, Model> readOnlyRepository,
     IValidationStrategy<CreateProductDTO> validationStrategy, IMapper mapper)
      : ICommandHandler<CreateProductCommand, ProductViewModel>
{
    public async Task<ProductViewModel> Handle(CreateProductCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        Validate(command.Input);

        var entityExist = await readOnlyRepository.GetAllMatchingAsync(x => x.Name == command.Input.Name);

        if (entityExist.Any())
        {
            throw new InvalidOperationException("Ya existe en la base de datos un producto con el mismo nombre.");
        }

        var entity = mapper.Map<Model>(command.Input);
        await writeOnlyRepository.AddAsync(entity);
        return mapper.Map<ProductViewModel>(entity);
    }

    private void Validate(CreateProductDTO entityDTO)
    {
        var validationResult = validationStrategy.Validate(entityDTO);
        if (!validationResult.IsValid) throw new BusinessValidationException(validationResult.Errors);
    }
}
