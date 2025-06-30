using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Application.BussinesProcess.Product.Command;
using OdontFlow.Application.BussinesProcess.Product.Query;
using OdontFlow.Application.Services.Contracts;
using OdontFlow.Domain.DTOs.Product;
using OdontFlow.Domain.ViewModel.Product;

namespace OdontFlow.Application.Services;

public class ProductsService(IMediator mediator) : IProductsService
{
    public async Task<ProductViewModel> CreateAsync(CreateProductDTO input)
    {
        var handler = mediator.GetCommandHandler<CreateProductCommand, ProductViewModel>();
        return await handler.Handle(new CreateProductCommand(input));
    }

    public async Task<ProductViewModel> DeleteAsync(Guid id)
    {
        var handler = mediator.GetCommandHandler<DeleteProductCommand, ProductViewModel>();
        return await handler.Handle(new DeleteProductCommand(id));
    }

    public async Task<IEnumerable<ProductViewModel>> GetAsync()
    {
        var handler = mediator.GetQueryHandler<GetProductsQuery, IEnumerable<ProductViewModel>>();
        return await handler.Handle(new GetProductsQuery());
    }

    public async Task<IEnumerable<ProductViewModel>> GetByCategoryAsync(PRODUCT_CATEGORY category)
    {
        var handler = mediator.GetQueryHandler<GetProductsByCategoryQuery, IEnumerable<ProductViewModel>>();
        return await handler.Handle(new GetProductsByCategoryQuery(category));
    }

    public async Task<IEnumerable<ProductViewModel>> GetByPlanIdAsync(Guid id)
    {
        var handler = mediator.GetQueryHandler<GetProductsByPlanIdQuery, IEnumerable<ProductViewModel>>();
        return await handler.Handle(new GetProductsByPlanIdQuery(id));
    }

    public async Task<IEnumerable<ProductViewModel>> GetWithoutPlanAsync()
    {
        var handler = mediator.GetQueryHandler<GetProductsWithoutPlanQuery, IEnumerable<ProductViewModel>>();
        return await handler.Handle(new GetProductsWithoutPlanQuery());
    }

    public  async Task<ProductViewModel> UpdateAsync(UpdateProductDTO input)
    {
        var handler = mediator.GetCommandHandler<UpdateProductCommand, ProductViewModel>();
        return await handler.Handle(new UpdateProductCommand(input));
    }
}
