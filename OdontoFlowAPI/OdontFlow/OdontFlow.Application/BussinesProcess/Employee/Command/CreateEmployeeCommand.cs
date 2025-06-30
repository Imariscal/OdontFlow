using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Employee;
using ClientModel = OdontFlow.Domain.Entities.Client;
using ViewModel = OdontFlow.Domain.ViewModel.Employee.EmployeeViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.Employee.CreateEmployeeDTO;

namespace OdontFlow.Application.BussinesProcess.Employee.Command;

public class CreateEmployeeCommand(CreateDTO input) : ICommand<ViewModel>
{
    public CreateDTO Input { get; set; } = input;
}
public class CreateEmployeeCommandHandler(
    IWriteOnlyRepository<Guid, Model> writeOnlyRepository,
    IReadOnlyRepository<Guid, Model> readOnlyRepository,
    IReadOnlyRepository<Guid, ClientModel> clientReadRepo,
    IMapper mapper
) : ICommandHandler<CreateEmployeeCommand, ViewModel>
{
    public async Task<ViewModel> Handle(CreateEmployeeCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(command.Input);

        var input = command.Input;

        var existingUser = await readOnlyRepository.GetAllMatchingAsync(e => e.Name == input.Name);
        if (existingUser.Any())
        {
            throw new InvalidOperationException("Ya existe en la base de datos un empleado con el mismo nombre.");
        }

        // Crear empleado
        var employee = mapper.Map<Model>(input);
        await writeOnlyRepository.AddAsync(employee);

       
        return mapper.Map<ViewModel>(employee);
    }
}
