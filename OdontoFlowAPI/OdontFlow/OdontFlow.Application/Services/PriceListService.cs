using ViewModel = OdontFlow.Domain.ViewModel.PriceList.PriceListViewModel;
using ViewItemModel = OdontFlow.Domain.ViewModel.PriceList.PriceListItemViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.PriceList.UpdatePriceListDTO;
using UpdateItemDTO = OdontFlow.Domain.DTOs.PriceList.PriceListItemUpdateDTO;
using CreateDTO = OdontFlow.Domain.DTOs.PriceList.CreatePriceListDTO;
using CreateItemDTO = OdontFlow.Domain.DTOs.PriceList.PriceListItemCreateDTO;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Application.Services.Contracts;  
using OdontFlow.Application.BussinesProcess.PriceList.Command;
using OdontFlow.Application.BussinesProcess.PriceList.Query;
using OdontFlow.Application.BussinesProcess.PriceListItem.Command;
namespace OdontFlow.Application.Services;

public class PriceListService(IMediator mediator) : IPriceListService
{
    public async Task<ViewModel> CreateAsync(CreateDTO input)
    {
        var handler = mediator.GetCommandHandler<CreatePriceListCommand, ViewModel>();
        return await handler.Handle(new CreatePriceListCommand(input));
    }

    public async Task<ViewItemModel> CreateItemAsync(CreateItemDTO input)
    {
        var handler = mediator.GetCommandHandler<CreatePriceListItemCommand, ViewItemModel>();
        return await handler.Handle(new CreatePriceListItemCommand(input));
    }

    public async Task<ViewModel> DeleteAsync(Guid id)
    {
        var handler = mediator.GetCommandHandler<DeletePriceListCommand, ViewModel>();
        return await handler.Handle(new DeletePriceListCommand(id));
    }

    public async Task<ViewItemModel> DeleteItemAsync(Guid id)
    {
        var handler = mediator.GetCommandHandler<DeletePriceListItemCommand, ViewItemModel>();
        return await handler.Handle(new DeletePriceListItemCommand(id));
    }

    public async Task<IEnumerable<ViewModel>> GetAsync(bool onlyActive = false)
    {
        var handler = mediator.GetQueryHandler<GetPriceListsQuery, IEnumerable<ViewModel>>();
        return await handler.Handle(new GetPriceListsQuery(onlyActive));
    }

    public async Task<IEnumerable<ViewItemModel>> GetProductListItemsAsync(Guid id)
    {
        var handler = mediator.GetQueryHandler<GetPriceListItemsQuery, IEnumerable<ViewItemModel>>();
        return await handler.Handle(new GetPriceListItemsQuery(id));
    }

    public async Task<ViewModel> UpdateAsync(UpdateDTO input)
    {
        var handler = mediator.GetCommandHandler<UpdatePriceListCommand, ViewModel>();
        return await handler.Handle(new UpdatePriceListCommand(input));
    }

    public async Task<ViewItemModel> UpdateItemAsync(UpdateItemDTO input)
    {
        var handler = mediator.GetCommandHandler<UpdatePriceListItemCommand, ViewItemModel>();
        return await handler.Handle(new UpdatePriceListItemCommand(input));
    }
}
