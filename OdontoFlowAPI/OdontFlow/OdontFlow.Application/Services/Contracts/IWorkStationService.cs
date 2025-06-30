using ViewModel = OdontFlow.Domain.ViewModel.WorkStation.WorkStationViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.WorkStation.UpdateWorkStationDTO;
using CreateDTO = OdontFlow.Domain.DTOs.WorkStation.CreateWorkStationDTO;
namespace OdontFlow.Application.Services.Contracts;
public interface IWorkStationService
{
    Task<IEnumerable<ViewModel>> GetAsync();
    Task<IEnumerable<ViewModel>> GetActiveAsync();
    Task<ViewModel> CreateAsync(CreateDTO input);
    Task<ViewModel> UpdateAsync(UpdateDTO input);
    Task<ViewModel> DeleteAsync(Guid id);
}