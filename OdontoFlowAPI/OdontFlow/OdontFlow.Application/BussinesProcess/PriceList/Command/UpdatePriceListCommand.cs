using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.BusinessRules.Base;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.PriceList;
using ViewModel = OdontFlow.Domain.ViewModel.PriceList.PriceListViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.PriceList.UpdatePriceListDTO;
using Mapster; 

namespace OdontFlow.Application.BussinesProcess.PriceList.Command;

public class UpdatePriceListCommand(UpdateDTO input) : ICommand<ViewModel>
{
    public UpdateDTO Input { get; set; } = input;
}

public class UpdatePriceListCommandHandler(
 IWriteOnlyRepository<Guid, Model> repository,
 IReadOnlyRepository<Guid, Model> readOnlyRepository, IMapper mapper,
 IValidationStrategy<UpdateDTO> validationStrategy) : ICommandHandler<UpdatePriceListCommand, ViewModel>
{
    public async Task<ViewModel> Handle(UpdatePriceListCommand command)
    {
        ArgumentNullException.ThrowIfNull(command.Input);
        ArgumentNullException.ThrowIfNull(command.Input.Id);   
        Validate(command.Input);
        var result = await readOnlyRepository.GetAsync(command.Input.Id) ?? throw new NotFoundException("List de precios no encontrada");
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
