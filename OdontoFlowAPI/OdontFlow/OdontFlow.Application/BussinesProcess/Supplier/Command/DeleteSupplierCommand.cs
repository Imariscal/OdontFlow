using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.Supplier;
using ViewModel = OdontFlow.Domain.ViewModel.Supplier.SupplierViewModel;

namespace OdontFlow.Application.BussinesProcess.Supplier.Command;
public class DeleteSupplierCommand(Guid id) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id;
}
public class DeleteSupplierCommandHandler(
      IWriteOnlyRepository<Guid, Model> repository, IMapper mapper,
      IReadOnlyRepository<Guid, Model> readOnlyRepository) : ICommandHandler<DeleteSupplierCommand, ViewModel>
{
    public async Task<ViewModel> Handle(DeleteSupplierCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var result = await readOnlyRepository.GetAllMatchingAsync(u => u.Id == command.Id);
        var entity = result.FirstOrDefault() ?? throw new NotFoundException("Proveedor NO encontrado");

        //TODO: Validar Producton con ordenes antes de eliminar.

        await repository.Remove(entity);
        return mapper.Map<ViewModel>(entity);
    }
}
