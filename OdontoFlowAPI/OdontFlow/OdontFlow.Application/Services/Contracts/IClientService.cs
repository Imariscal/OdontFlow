using ViewModel = OdontFlow.Domain.ViewModel.Client.ClientViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.Client.UpdateClientDTO;
using CreateDTO = OdontFlow.Domain.DTOs.Client.CreateClientDTO;

namespace OdontFlow.Application.Services.Contracts;
public interface IClientService
{
    Task<IEnumerable<ViewModel>> GetAsync();
    Task<IEnumerable<ViewModel>> GetActiveAsync();
    Task<ViewModel> CreateAsync(CreateDTO input);
    Task<ViewModel> UpdateAsync(UpdateDTO input);
    Task<ViewModel> DeleteAsync(Guid id);
}
