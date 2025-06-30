using ViewModel = OdontFlow.Domain.ViewModel.WorkPlan.WorkPlanViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.WorksPlan.UpdateWorkPlanDTO;
using CreateDTO = OdontFlow.Domain.DTOs.WorksPlan.CreateWorkPlanDTO; 
 

namespace OdontFlow.Application.Services.Contracts;

public interface IWorkPlanService
{
    Task<IEnumerable<ViewModel>> GetAsync();
    Task<ViewModel> CreateAsync(CreateDTO input);
    Task<ViewModel> UpdateAsync(UpdateDTO input);
    Task<ViewModel> DeleteAsync(Guid id);
}
