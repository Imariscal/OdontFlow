
using OdontFlow.Domain.ViewModel.StationWork;
using ViewModel = OdontFlow.Domain.ViewModel.Lab.StationWorkSummaryViewModel;

namespace OdontFlow.Application.Services.Contracts;

public interface ILabService
{
    Task<ViewModel> GetAsync();

    Task<IEnumerable<StationWorkDetailViewModel>> GetWorkStationDetail(Guid Id);

    Task<StationWorkDetailViewModel> ProcessWorkStationDetail(Guid Id);

    Task<StationWorkDetailViewModel> CompleteWorkStationDetail(Guid Id);

    Task<StationWorkDetailViewModel> RejectWorkStationDetail(Guid Id, string message);

    Task<StationWorkDetailViewModel> BlockWorkStationDetail(Guid Id, string message);

    Task<StationWorkDetailViewModel> UnBlockWorkStationDetail(Guid Id);
}
