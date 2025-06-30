using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.CrossCutting.Exceptions;
using OdontFlow.Domain.Repositories.Base; 
using Model = OdontFlow.Domain.Entities.Product;
using ViewModel = OdontFlow.Domain.ViewModel.Product.ProductViewModel;

namespace OdontFlow.Application.BussinesProcess.Product.Command;

public class DeleteProductCommand(Guid id) : ICommand<ViewModel>
{
    public Guid Id { get; set; } = id;
}
public class DeleteProductCommandHandler(
      IWriteOnlyRepository<Guid, Model> repository, IMapper mapper,
      IReadOnlyRepository<Guid, Model> readOnlyRepository) : ICommandHandler<DeleteProductCommand, ViewModel>
{
    public async Task<ViewModel> Handle(DeleteProductCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var result = await readOnlyRepository.GetAllMatchingAsync(u => u.Id == command.Id);
        var entity = result.FirstOrDefault() ?? throw new NotFoundException("Producto NO encontrado");

        //TODO: Validar Producton con ordenes antes de eliminar.

        await repository.Remove(entity);
        return mapper.Map<ViewModel>(entity);
    }
}