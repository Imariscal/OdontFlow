using ViewModel = OdontFlow.Domain.ViewModel.Supplier.SupplierViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.Supplier.UpdateSupplierDTO;
using CreateDTO = OdontFlow.Domain.DTOs.Supplier.CreateSupplierDTO; 

namespace OdontFlow.Application.Services.Contracts;

public interface ISupplierService{
    Task<IEnumerable<ViewModel>> GetAsync();
    Task<ViewModel> CreateAsync(CreateDTO input);
    Task<ViewModel> UpdateAsync(UpdateDTO input);
    Task<ViewModel> DeleteAsync(Guid id);
}
