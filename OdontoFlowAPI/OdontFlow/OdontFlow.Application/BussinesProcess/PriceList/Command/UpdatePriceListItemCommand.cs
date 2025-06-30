using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions; 
using OdontFlow.Domain.Repositories.Base; 
using Model = OdontFlow.Domain.Entities.PriceListItem;
using ViewModel = OdontFlow.Domain.ViewModel.PriceList.PriceListItemViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.PriceList.PriceListItemUpdateDTO;
using Mapster;

namespace OdontFlow.Application.BussinesProcess.PriceListItem.Command;
public class UpdatePriceListItemCommand(UpdateDTO input) : ICommand<ViewModel>
{
    public UpdateDTO Input { get; set; } = input;
}

public class UpdatePriceListItemCommandHandler(
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IWriteOnlyRepository<Guid, Model> writeOnlyRepository, 
    IMapper mapper)
    : ICommandHandler<UpdatePriceListItemCommand, ViewModel>
{
    public async Task<ViewModel> Handle(UpdatePriceListItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command); 

        var entity = await readOnlyRepository.GetAsync(command.Input.Id)
                     ?? throw new NotFoundException($"El registro con ID {command.Input.Id} no existe."); 

        command.Input.Adapt(entity);
        await writeOnlyRepository.Modify(entity);
        return mapper.Map<ViewModel>(entity);
    }  
}
