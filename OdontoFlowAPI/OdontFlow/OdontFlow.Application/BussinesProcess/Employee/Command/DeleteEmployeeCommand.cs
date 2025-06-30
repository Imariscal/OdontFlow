using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Employee;
using ViewModel = OdontFlow.Domain.ViewModel.Employee.EmployeeViewModel;

namespace OdontFlow.Application.BussinesProcess.Employee.Command;

public class DeleteEmployeeCommand(Guid id) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id;
}

public class DeleteEmployeeCommandHandler(
    IWriteOnlyRepository<Guid, Model> writeRepo,
    IReadOnlyRepository<Guid, Model> readRepo,
    IMapper mapper
) : ICommandHandler<DeleteEmployeeCommand, ViewModel>
{
    public async Task<ViewModel> Handle(DeleteEmployeeCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var entity = await readRepo.GetAsync(command.Id)
            ?? throw new NotFoundException("Empleado no encontrado.");

        await writeRepo.Remove(entity);

        return mapper.Map<ViewModel>(entity);
    }
}
