using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.WorkStation;
using ViewModel = OdontFlow.Domain.ViewModel.WorkStation.WorkStationViewModel;

namespace OdontFlow.Application.BussinesProcess.WorkStation.Command;

public class DeleteWorkStationCommand(Guid id) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id;
}

public class DeleteWorkStationCommandHandler(
      IWriteOnlyRepository<Guid, Model> repository, IMapper mapper,
      IReadOnlyRepository<Guid, Model> readOnlyRepository) : ICommandHandler<DeleteWorkStationCommand, ViewModel>
{
    public async Task<ViewModel> Handle(DeleteWorkStationCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var result = await readOnlyRepository.GetAllMatchingAsync(u => u.Id == command.Id);
        var entity = result.FirstOrDefault() ?? throw new NotFoundException("Estación de trabajo NO encontrado");

        //TODO: Validar checar ordenes existentes con ordenes.

        await repository.Remove(entity);
        return mapper.Map<ViewModel>(entity);
    }
}


