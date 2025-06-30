using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.BusinessRules.Base;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.PriceList;
using ViewModel = OdontFlow.Domain.ViewModel.PriceList.PriceListViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.PriceList.CreatePriceListDTO;

namespace OdontFlow.Application.BussinesProcess.PriceList.Command;

public class CreatePriceListCommand(CreateDTO input) : ICommand<ViewModel>
{
    public CreateDTO Input { get; set; } = input;
}

public class CreatePriceListCommandHandler(
     IWriteOnlyRepository<Guid, Model> writeOnlyRepository,
     IReadOnlyRepository<Guid, Model> readOnlyRepository,
     IValidationStrategy<CreateDTO> validationStrategy, IMapper mapper)
      : ICommandHandler<CreatePriceListCommand, ViewModel>
{
    public async Task<ViewModel> Handle(CreatePriceListCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        Validate(command.Input);

        var entityExist = await readOnlyRepository.GetAllMatchingAsync(x => x.Name == command.Input.Name);

        if (entityExist.Any())
        {
            throw new InvalidOperationException("Ya existe en la base de datos una lista con el mismo nombre.");
        }

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
