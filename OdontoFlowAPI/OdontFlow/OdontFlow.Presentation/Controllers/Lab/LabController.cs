using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontFlow.API.Execution;
using OdontFlow.API.Execution.Answers.Contracts;
using OdontFlow.Application.Services.Contracts;
using OdontFlow.Domain.ViewModel.StationWork;
using System.Net;
using ViewModel = OdontFlow.Domain.ViewModel.Lab.StationWorkSummaryViewModel;

namespace OdontFlow.Presentation.Controllers.Lab;

[Produces("application/json")]
[Route("api/v1/[controller]")]
[Authorize]
public class LabController(ILabService service) : Controller
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get()
    {
        async Task<ViewModel> predicate() => await service.GetAsync();
        var response = await SafeExecutor<ViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpGet("{id}/workstationDetails")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetWorkstationDetails(Guid id)
    {
        async Task<IEnumerable<StationWorkDetailViewModel>> predicate() => await service.GetWorkStationDetail(id);
        var response = await SafeExecutor<IEnumerable<StationWorkDetailViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }


    [HttpPost("{id}/processOrder")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> ProcessOrder(Guid id)
    {
        async Task<StationWorkDetailViewModel> predicate() => await service.ProcessWorkStationDetail(id);
        var response = await SafeExecutor<StationWorkDetailViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPost("{id}/completeOrder")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> CompleteOrder(Guid id)
    {
        async Task<StationWorkDetailViewModel> predicate() => await service.CompleteWorkStationDetail(id);
        var response = await SafeExecutor<StationWorkDetailViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }


    [HttpPost("{id}/rejectOrder")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> rejectOrder([FromRoute] Guid id, [FromBody] string message)
    {
        async Task<StationWorkDetailViewModel> predicate() => await service.RejectWorkStationDetail(id, message);
        var response = await SafeExecutor<StationWorkDetailViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPost("{id}/blockOrder")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> BlockOrder([FromRoute] Guid id, [FromBody] string message)
    {
        async Task<StationWorkDetailViewModel> predicate() => await service.BlockWorkStationDetail(id, message);
        var response = await SafeExecutor<StationWorkDetailViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPost("{id}/unBlockOrder")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> UnBlockOrder([FromRoute] Guid id)
    {
        async Task<StationWorkDetailViewModel> predicate() => await service.UnBlockWorkStationDetail(id);
        var response = await SafeExecutor<StationWorkDetailViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }



    protected ActionResult ProcessResponse<T>(IAnswerBase<T> response) where T : class
    {
        if (response.Success) return Ok(response);
        else return BadRequest(response);
    }
}
