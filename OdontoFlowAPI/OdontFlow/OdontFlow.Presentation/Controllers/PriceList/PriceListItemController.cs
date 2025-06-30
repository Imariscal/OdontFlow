using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModel = OdontFlow.Domain.ViewModel.PriceList.PriceListItemViewModel;
using UpdateDTO = OdontFlow.Domain.DTOs.PriceList.PriceListItemUpdateDTO;
using CreateDTO = OdontFlow.Domain.DTOs.PriceList.PriceListItemCreateDTO;
using OdontFlow.Application.Services.Contracts;
using OdontFlow.API.Execution.Answers.Contracts;
using OdontFlow.API.Execution;
using System.Net;

namespace OdontFlow.Presentation.Controllers.PriceList;

[Produces("application/json")]
[Route("api/v1/[controller]")]
[Authorize]
public class PriceListItemController(IPriceListService service) : Controller
{
    [HttpGet("{id}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        async Task<IEnumerable<ViewModel>> predicate() => await service.GetProductListItemsAsync(id);
        var response = await SafeExecutor<IEnumerable<ViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Post([FromBody] CreateDTO input)
    {
        async Task<ViewModel> predicate() => await service.CreateItemAsync(input);
        var response = await SafeExecutor<ViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Put([FromBody] UpdateDTO input)
    {
        async Task<ViewModel> predicate() => await service.UpdateItemAsync(input);
        var response = await SafeExecutor<ViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        async Task<ViewModel> predicate() => await service.DeleteItemAsync(id);
        var response = await SafeExecutor<ViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    protected ActionResult ProcessResponse<T>(IAnswerBase<T> response) where T : class
    {
        if (response.Success) return Ok(response);
        else return BadRequest(response);
    }
}
