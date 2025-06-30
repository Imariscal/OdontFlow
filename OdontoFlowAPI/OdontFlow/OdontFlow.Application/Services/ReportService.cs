using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Application.BussinesProcess.Reports.Query;
using OdontFlow.Application.BussinesProcess.StationWork.Query;
using OdontFlow.Application.Services.Contracts;
using OdontFlow.Catalogos.Retail.Application.BusinessProcess.Base;
using OdontFlow.CrossCutting.Common;
using OdontFlow.Domain.ViewModel.Order;
using OdontFlow.Domain.ViewModel.Report;

namespace OdontFlow.Application.Services;

public class ReportService(IMediator mediator) : IReportService
{
    public async Task<ProductivityReportViewModel> GetAsync(DateTime startDate, DateTime endDate, int mode)
    {
        var handler = mediator.GetQueryHandler<GetFilteredProductivityReportQuery, ProductivityReportViewModel>();
        return await handler.Handle(new GetFilteredProductivityReportQuery(startDate, endDate, mode));
    }

    public async Task<CommissionOrdersReportViewModel> GetCommissionReportQuery(OrderAdvancedFilterViewModel input)
    {
        var handler = mediator.GetQueryHandler<GetCommissionOrdersReportQuery, CommissionOrdersReportViewModel>();
        return await handler.Handle(new GetCommissionOrdersReportQuery(input));
    }

    public async Task<PagedResult<OrderViewModel>> GetDebtOrdersQueryFilterAsync(OrderAdvancedFilterViewModel input)
    {
        var handler = mediator.GetQueryHandler<GetDebtOrdersQuery, PagedResult<OrderViewModel>>();
        return await handler.Handle(new GetDebtOrdersQuery(input));
    }

    public async Task<PagedWithSummaryResult<OrderViewModel>> GetDebtOrdersWithSummaryAsync(OrderAdvancedFilterViewModel input)
    {
        var handler = mediator.GetQueryHandler<GetDebtOrdersWithSummaryQuery, PagedWithSummaryResult<OrderViewModel>>();
        return await handler.Handle(new GetDebtOrdersWithSummaryQuery(input));
    }

    public async Task<PagedResult<OrderViewModel>> GetOrdersByAdvancedFilterAsync(OrderAdvancedFilterViewModel input)
    {
        var handler = mediator.GetQueryHandler<GetOrdersByAdvancedFilterQuery, PagedResult<OrderViewModel>>();
        return await handler.Handle(new GetOrdersByAdvancedFilterQuery(input));
    }

    public async Task<PagedResult<ClientOrdersSummaryViewModel>> GetPaymentAndDebtsDetailsQuery(OrderAdvancedFilterViewModel input)
    {
        var handler = mediator.GetQueryHandler<GetPaymentAndDebtsDetailsQuery, PagedResult<ClientOrdersSummaryViewModel>>();
        return await handler.Handle(new GetPaymentAndDebtsDetailsQuery(input));
    }

    public async Task<PagedResult<OrderViewModel>> GetPaymentsByAdvancedFilterAsync(OrderAdvancedFilterViewModel input)
    {
        var handler = mediator.GetQueryHandler<GetOrdersPaymentsByAdvancedFilterQuery, PagedResult<OrderViewModel>>();
        return await handler.Handle(new GetOrdersPaymentsByAdvancedFilterQuery(input));
    }

    public async Task<PagedResult<WorkedPiecesReportViewModel>> GetWorkedPiecesReportQuery(OrderAdvancedFilterViewModel input)
    {
        var handler = mediator.GetQueryHandler<GetWorkedPiecesReportQuery, PagedResult<WorkedPiecesReportViewModel>>();
        return await handler.Handle(new GetWorkedPiecesReportQuery(input));
    }

    public async Task<List<OrderViewModel>> ExportDebtOrdersAsync(OrderAdvancedFilterViewModel input)
    {
        var handler = mediator.GetQueryHandler<GetDebtOrdersExportQuery, List<OrderViewModel>>();
        return await handler.Handle(new GetDebtOrdersExportQuery(input));
    }
}
