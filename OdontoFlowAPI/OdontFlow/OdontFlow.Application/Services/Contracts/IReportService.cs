

using OdontFlow.Application.BussinesProcess.Reports.Query;
using OdontFlow.CrossCutting.Common;
using OdontFlow.Domain.ViewModel.Order;
using OdontFlow.Domain.ViewModel.Report;

namespace OdontFlow.Application.Services.Contracts;

public interface IReportService
{
    Task<ProductivityReportViewModel> GetAsync(DateTime startDate, DateTime endDate, int mode);
    Task<PagedResult<OrderViewModel>> GetOrdersByAdvancedFilterAsync(OrderAdvancedFilterViewModel input);
    Task<PagedResult<OrderViewModel>> GetPaymentsByAdvancedFilterAsync(OrderAdvancedFilterViewModel input);
    Task<PagedResult<OrderViewModel>> GetDebtOrdersQueryFilterAsync(OrderAdvancedFilterViewModel input);
    Task<PagedResult<WorkedPiecesReportViewModel>> GetWorkedPiecesReportQuery(OrderAdvancedFilterViewModel input);
    Task<PagedResult<ClientOrdersSummaryViewModel>> GetPaymentAndDebtsDetailsQuery(OrderAdvancedFilterViewModel input);
    Task<CommissionOrdersReportViewModel> GetCommissionReportQuery(OrderAdvancedFilterViewModel input);
    Task<PagedWithSummaryResult<OrderViewModel>> GetDebtOrdersWithSummaryAsync(OrderAdvancedFilterViewModel input);
    Task<List<OrderViewModel>> ExportDebtOrdersAsync(OrderAdvancedFilterViewModel input);
}
