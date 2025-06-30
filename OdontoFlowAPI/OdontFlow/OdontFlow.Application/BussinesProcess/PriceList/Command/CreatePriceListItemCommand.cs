using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.PriceListItem;
using ViewModel = OdontFlow.Domain.ViewModel.PriceList.PriceListItemViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.PriceList.PriceListItemCreateDTO;
using Mapster;

namespace OdontFlow.Application.BussinesProcess.PriceListItem.Command;

public class CreatePriceListItemCommand(CreateDTO input) : ICommand<ViewModel>
{
    public CreateDTO Input { get; set; } = input;
}

public class CreatePriceListItemCommandHandler(
    IWriteOnlyRepository<Guid, Model> writeOnlyRepository,
    IReadOnlyRepository<Guid, Model> readOnlyRepository, 
    IMapper mapper)
    : ICommandHandler<CreatePriceListItemCommand, ViewModel>
{
    public async Task<ViewModel> Handle(CreatePriceListItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        //Validate(command.Input)
        var exists = await readOnlyRepository.GetAllMatchingAsync(x =>
            x.ProductId == command.Input.ProductId && x.PriceListId == command.Input.PriceListId);

        if (exists.Any())
        {
            var item = exists.FirstOrDefault();

            if (item!.Deleted)
            {
                item.Deleted = false;
                item.Price = command.Input.Price;
                await writeOnlyRepository.Modify(item);
                return mapper.Map<ViewModel>(item);
            } 
            else
            {
                throw new InvalidOperationException("Ya existe el producto seleccionado en la lista.");
            }

        }

        var entity = mapper.Map<Model>(command.Input);
        await writeOnlyRepository.AddAsync(entity);
        return mapper.Map<ViewModel>(entity);
    }

    //private void Validate(CreatePriceListItemRequest dto)
    //{
    //    var result = validationStrategy.Validate(dto);
    //    if (!result.IsValid) throw new BusinessValidationException(result.Errors);
    //}
}
