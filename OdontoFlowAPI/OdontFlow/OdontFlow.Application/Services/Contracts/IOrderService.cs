using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.Order.CreateOrderDTO;
using UpdateDTO = OdontFlow.Domain.DTOs.Order.UpdateOrderDTO;
using OdontFlow.CrossCutting.Common;
using OdontFlow.Domain.Entities;
using OdontFlow.Domain.DTOs.Order;

namespace OdontFlow.Application.Services.Contracts;

public interface IOrderService
{
    Task<PagedResult<ViewModel>> GetAsync(GetPagedOrdersQuery query);
    Task<ViewModel> GetByIdAsync(Guid id);
    Task<PagedResult<ViewModel>> GetByFilterAsync(string? search, int page, int pageSize);
    Task<ViewModel> CreateAsync(CreateDTO input);
    Task<ViewModel> UpdateAsync(UpdateDTO input);
    Task<ViewModel> DeleteAsync(Guid id);
    Task<ViewModel> DeleteOrdenItemAsync(Guid id);
    Task<ViewModel> ConfirmOrderAsync(Guid id);
    Task<ViewModel> DeliverOrdenAsync(Guid id);
    Task<byte[]> GeneratePdf(Guid id);
    Task<byte[]> GenerateRemisionZirconiaPdf(Guid id);
    Task<byte[]> GenerateRemisionGammaPdf(Guid id);
}
