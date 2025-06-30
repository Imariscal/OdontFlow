using ViewModel = OdontFlow.Domain.ViewModel.Employee.EmployeeViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.Employee.CreateEmployeeDTO;
using UpdateDTO = OdontFlow.Domain.DTOs.Employee.UpdateEmployeeDTO;

namespace OdontFlow.Application.Services.Contracts;

public interface IEmployeeService
{
    Task<IEnumerable<ViewModel>> GetAsync();
    Task<IEnumerable<ViewModel>> GetActiveAsync();
    Task<IEnumerable<ViewModel>> GetSalesEmployeeActiveAsync();
    Task<ViewModel> CreateAsync(CreateDTO input);
    Task<ViewModel> UpdateAsync(UpdateDTO input);
    Task<ViewModel> DeleteAsync(Guid id);
}