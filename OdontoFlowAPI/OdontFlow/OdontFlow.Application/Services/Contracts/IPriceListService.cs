using ViewModel = OdontFlow.Domain.ViewModel.PriceList.PriceListViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.PriceList.UpdatePriceListDTO;
using UpdateItemDTO = OdontFlow.Domain.DTOs.PriceList.PriceListItemUpdateDTO;
using CreateDTO = OdontFlow.Domain.DTOs.PriceList.CreatePriceListDTO;
using CreateItemDTO = OdontFlow.Domain.DTOs.PriceList.PriceListItemCreateDTO;
using ViewItemModel = OdontFlow.Domain.ViewModel.PriceList.PriceListItemViewModel;
namespace OdontFlow.Application.Services.Contracts;

public interface IPriceListService
{
    Task<IEnumerable<ViewModel>> GetAsync(bool onlyActive = false);
    Task<IEnumerable<ViewItemModel>> GetProductListItemsAsync(Guid id);
    Task<ViewModel> CreateAsync(CreateDTO input);
    Task<ViewModel> UpdateAsync(UpdateDTO input);
    Task<ViewModel> DeleteAsync(Guid id);
    Task<ViewItemModel> CreateItemAsync(CreateItemDTO input);
    Task<ViewItemModel> UpdateItemAsync(UpdateItemDTO input);
    Task<ViewItemModel> DeleteItemAsync(Guid id);
}
