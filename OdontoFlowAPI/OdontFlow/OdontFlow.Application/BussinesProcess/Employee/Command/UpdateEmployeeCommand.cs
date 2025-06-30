using MapsterMapper;
using Mapster;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Entities;
using OdontFlow.Domain.Repositories.Base;
using UpdateDTO = OdontFlow.Domain.DTOs.Employee.UpdateEmployeeDTO;
using Model = OdontFlow.Domain.Entities.Employee;
using ClientModel = OdontFlow.Domain.Entities.Client;
using ViewModel = OdontFlow.Domain.ViewModel.Employee.EmployeeViewModel;
using ZXing;
namespace OdontFlow.Application.BussinesProcess.Employee.Command;
public class UpdateEmployeeCommand(UpdateDTO input) : ICommand<ViewModel>
{
    public UpdateDTO Input { get; set; } = input;
}
public class UpdateEmployeeCommandHandler(
    IWriteOnlyRepository<Guid, Model> writeRepo,
    IReadOnlyRepository<Guid, Model> readRepo,
    IReadOnlyRepository<Guid, ClientModel> clientReadRepo,
    IMapper mapper
) : ICommandHandler<UpdateEmployeeCommand, ViewModel>
{
    public async Task<ViewModel> Handle(UpdateEmployeeCommand command)
    {
        ArgumentNullException.ThrowIfNull(command.Input);
        ArgumentNullException.ThrowIfNull(command.Input.Id);

        var input = command.Input;

        var employee = await readRepo.GetAsync(input.Id)
            ?? throw new NotFoundException("Empleado no encontrado.");

        input.Adapt(employee);
        await writeRepo.Modify(employee);



        return mapper.Map<ViewModel>(employee);
    }
}
