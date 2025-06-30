using ViewModel = OdontFlow.Domain.ViewModel.Supplier.SupplierViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.Supplier.UpdateSupplierDTO;
using CreateDTO = OdontFlow.Domain.DTOs.Supplier.CreateSupplierDTO;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Application.Services.Contracts; 
using OdontFlow.Application.BussinesProcess.Supplier.Command;
using OdontFlow.Application.BussinesProcess.Supplier.Query;
using OdontFlow.Application.BussinesProcess.Product.Command;

namespace OdontFlow.Application.Services;

public class SupplierService(IMediator mediator) : ISupplierService
{
    public async Task<ViewModel> CreateAsync(CreateDTO input)
    {
        var handler = mediator.GetCommandHandler<CreateSupplierCommand, ViewModel>();
        return await handler.Handle(new CreateSupplierCommand(input));
    }

    public async Task<ViewModel> DeleteAsync(Guid id)
    {
        var handler = mediator.GetCommandHandler<DeleteSupplierCommand, ViewModel>();
        return await handler.Handle(new DeleteSupplierCommand(id));
    }

    public async Task<IEnumerable<ViewModel>> GetAsync()
    {
        var handler = mediator.GetQueryHandler<GetSuppliersQuery, IEnumerable<ViewModel>>();
        return await handler.Handle(new GetSuppliersQuery());
    }

    public async Task<ViewModel> UpdateAsync(UpdateDTO input)
    {
        var handler = mediator.GetCommandHandler<UpdateSupplierCommand, ViewModel>();
        return await handler.Handle(new UpdateSupplierCommand(input));
    }
}
