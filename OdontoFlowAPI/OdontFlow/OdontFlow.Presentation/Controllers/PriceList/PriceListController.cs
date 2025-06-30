using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModel = OdontFlow.Domain.ViewModel.PriceList.PriceListViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.PriceList.UpdatePriceListDTO;
using CreateDTO = OdontFlow.Domain.DTOs.PriceList.CreatePriceListDTO;
using OdontFlow.Application.Services.Contracts;
using OdontFlow.API.Execution.Answers.Contracts;
using OdontFlow.API.Execution;
using System.Net;

namespace OdontFlow.Presentation.Controllers.PriceList;

[Produces("application/json")]
[Route("api/v1/[controller]")]
[Authorize]
public class PriceListController(IPriceListService service) : Controller
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get()
    {
        async Task<IEnumerable<ViewModel>> predicate() => await service.GetAsync();
        var response = await SafeExecutor<IEnumerable<ViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }


    [HttpGet("GetOnlyActive")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetOnlyActive()
    {
        async Task<IEnumerable<ViewModel>> predicate() => await service.GetAsync(true);
        var response = await SafeExecutor<IEnumerable<ViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }


    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Post([FromBody] CreateDTO input)
    {
        async Task<ViewModel> predicate() => await service.CreateAsync(input);
        var response = await SafeExecutor<ViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Put([FromBody] UpdateDTO input)
    {
        async Task<ViewModel> predicate() => await service.UpdateAsync(input);
        var response = await SafeExecutor<ViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        async Task<ViewModel> predicate() => await service.DeleteAsync(id);
        var response = await SafeExecutor<ViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    protected ActionResult ProcessResponse<T>(IAnswerBase<T> response) where T : class
    {
        if (response.Success) return Ok(response);
        else return BadRequest(response);
    }
}
