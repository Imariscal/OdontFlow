using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Order;
using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;
using ClientModel = OdontFlow.Domain.Entities.Client;

namespace OdontFlow.Application.BussinesProcess.Order.Command;

public class DeleteOrderCommand(Guid id) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id;
}

public class DeleteOrderCommandHandler(
    IWriteOnlyRepository<Guid, Model> repository,
    IReadOnlyRepository<Guid, Model> readRepository,
        IReadOnlyRepository<Guid, ClientModel> clientRepository,
    IMapper mapper
) : ICommandHandler<DeleteOrderCommand, ViewModel>
{
    public async Task<ViewModel> Handle(DeleteOrderCommand command)
    {
        var entity = await readRepository.GetAsync(command.Id)
            ?? throw new NotFoundException("Orden no encontrada.");

        await repository.Remove(entity);

        entity.Client = await clientRepository.GetAsync(entity.ClientId);
        return mapper.Map<ViewModel>(entity);
    }
}
