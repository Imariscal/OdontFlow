using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts; 
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.PriceListItem;
using ViewModel = OdontFlow.Domain.ViewModel.PriceList.PriceListItemViewModel;

namespace OdontFlow.Application.BussinesProcess.PriceList.Command;

public class DeletePriceListItemCommand(Guid id) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id;
}

public class DeletePriceListItemCommandHandler(
      IWriteOnlyRepository<Guid, Model> repository, IMapper mapper,
      IReadOnlyRepository<Guid, Model> readOnlyRepository) : ICommandHandler<DeletePriceListItemCommand, ViewModel>
{
    public async Task<ViewModel> Handle(DeletePriceListItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var result = await readOnlyRepository.GetAllMatchingAsync(u => u.Id == command.Id);
        var entity = result.FirstOrDefault() ?? throw new NotFoundException("Lista de Precios NO encontrado");

        //TODO: Validar checar listas de precios existentes

        await repository.Remove(entity);
        return mapper.Map<ViewModel>(entity);
    }
}