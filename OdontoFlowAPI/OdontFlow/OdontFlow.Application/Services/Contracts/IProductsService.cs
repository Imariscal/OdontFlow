using OdontFlow.Domain.DTOs.Product;
using OdontFlow.Domain.ViewModel.Product;

namespace OdontFlow.Application.Services.Contracts;

public interface IProductsService
{
    Task<IEnumerable<ProductViewModel>> GetAsync();

    Task<IEnumerable<ProductViewModel>> GetByCategoryAsync(PRODUCT_CATEGORY category);

    Task<IEnumerable<ProductViewModel>> GetWithoutPlanAsync();

    Task<IEnumerable<ProductViewModel>> GetByPlanIdAsync(Guid id);

    Task<ProductViewModel> CreateAsync(CreateProductDTO input);

    Task<ProductViewModel> UpdateAsync(UpdateProductDTO input);

    Task<ProductViewModel> DeleteAsync(Guid id);
}
