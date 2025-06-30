using ViewModel = OdontFlow.Domain.ViewModel.OrderPayment.OrderPaymentViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.OrderPayment.CreateOrderPaymentDTO;
using UpdateDTO = OdontFlow.Domain.DTOs.OrderPayment.UpdateOrderPaymentDTO;
 
namespace OdontFlow.Application.Services.Contracts;

public interface IOrderPaymentService
{
    Task<IEnumerable<ViewModel>> GetAsync(Guid id);

    Task<ViewModel> CreateAsync(CreateDTO input);
    Task<ViewModel> UpdateAsync(UpdateDTO input);
    Task<ViewModel> DeleteAsync(Guid id);
}
