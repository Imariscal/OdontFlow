using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontFlow.API.Execution;
using OdontFlow.API.Execution.Answers.Contracts; 
using OdontFlow.Application.Services.Contracts;
using OdontFlow.CrossCutting.Common;
using OdontFlow.Domain.ViewModel.Order;
using OdontFlow.Domain.ViewModel.Report; 
using System.Net;

namespace OdontFlow.Presentation.Controllers.Lab;

[Produces("application/json")]
[Route("api/v1/[controller]")]
[Authorize]
public class ReportController(IReportService service) : Controller
{
    [HttpGet("resume")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetResume([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        async Task<ProductivityReportViewModel> predicate() => await service.GetAsync(startDate, endDate,1);
        var response = await SafeExecutor<ProductivityReportViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpGet("stationDetail")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetStationDetail([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        async Task<ProductivityReportViewModel> predicate() => await service.GetAsync(startDate, endDate, 1);
        var response = await SafeExecutor<ProductivityReportViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpGet("employeeDetail")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetEmployeeDetail([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        async Task<ProductivityReportViewModel> predicate() => await service.GetAsync(startDate, endDate, 2);
        var response = await SafeExecutor<ProductivityReportViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }
      
    [HttpPost("ordersByAdvancedFilter")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetOrdersByAdvancedFilter([FromBody] OrderAdvancedFilterViewModel query)
    {
        async Task<PagedResult<OrderViewModel>> predicate() => await service.GetOrdersByAdvancedFilterAsync(query);
        var response = await SafeExecutor<PagedResult<OrderViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPost("paymentsByAdvancedFilter")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetPaymentsByAdvancedFilter([FromBody] OrderAdvancedFilterViewModel query)
    {
        async Task<PagedResult<OrderViewModel>> predicate() => await service.GetPaymentsByAdvancedFilterAsync(query);
        var response = await SafeExecutor<PagedResult<OrderViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }


    [HttpPost("debtsByAdvancedFilter")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetDebtsByAdvancedFilter([FromBody] OrderAdvancedFilterViewModel query)
    {
        async Task<PagedResult<OrderViewModel>> predicate() => await service.GetDebtOrdersQueryFilterAsync(query);
        var response = await SafeExecutor<PagedResult<OrderViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPost("workedPiecesByAdvancedFilter")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetWorkedPiecesByAdvancedFilter([FromBody] OrderAdvancedFilterViewModel query)
    {
        async Task<PagedResult<WorkedPiecesReportViewModel>> predicate() => await service.GetWorkedPiecesReportQuery(query);
        var response = await SafeExecutor<PagedResult<WorkedPiecesReportViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPost("paymentAndDebtsByAdvancedFilter")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetPaymentAndDebtsDetailsQuery([FromBody] OrderAdvancedFilterViewModel query)
    {
        async Task<PagedResult<ClientOrdersSummaryViewModel>> predicate() => await service.GetPaymentAndDebtsDetailsQuery(query);
        var response = await SafeExecutor<PagedResult<ClientOrdersSummaryViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPost("commisionByAdvancedFilter")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetComissionDetailsQuery([FromBody] OrderAdvancedFilterViewModel query)
    {
        async Task<CommissionOrdersReportViewModel> predicate() => await service.GetCommissionReportQuery(query);
        var response = await SafeExecutor<CommissionOrdersReportViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPost("debtOrdersWithSummary")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetDebtOrdersWithSummary([FromBody] OrderAdvancedFilterViewModel query)
    {
        async Task<PagedWithSummaryResult<OrderViewModel>> predicate() =>
            await service.GetDebtOrdersWithSummaryAsync(query);

        var response = await SafeExecutor<PagedWithSummaryResult<OrderViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPost("debt/export")]
    [ProducesResponseType(typeof(List<OrderViewModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ExportDebtOrders([FromBody] OrderAdvancedFilterViewModel query)
    {
        async Task<List<OrderViewModel>> predicate() =>
            await service.ExportDebtOrdersAsync(query);

        var response = await SafeExecutor<List<OrderViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    protected ActionResult ProcessResponse<T>(IAnswerBase<T> response) where T : class
    {
        return response.Success ? Ok(response) : BadRequest(response);
    }
}
