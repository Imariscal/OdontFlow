using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Client;
using ViewModel = OdontFlow.Domain.ViewModel.Client.ClientViewModel;

namespace OdontFlow.Application.BussinesProcess.Client.Command;

public class DeleteClientCommand(Guid id) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id;
}

public class DeleteClientCommandHandler(
      IWriteOnlyRepository<Guid, Model> repository, IMapper mapper,
      IReadOnlyRepository<Guid, Model> readOnlyRepository) : ICommandHandler<DeleteClientCommand, ViewModel>
{
    public async Task<ViewModel> Handle(DeleteClientCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var result = await readOnlyRepository.GetAllMatchingAsync(u => u.Id == command.Id);
        var entity = result.FirstOrDefault() ?? throw new NotFoundException("Client NO encontrado");

        //TODO: Validar checar ordenes existentes con ordenes.

        await repository.Remove(entity);
        return mapper.Map<ViewModel>(entity);
    }
}


