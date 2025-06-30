using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModel = OdontFlow.Domain.ViewModel.Order.OrderViewModel;
using CreateDTO = OdontFlow.Domain.DTOs.Order.CreateOrderDTO;
using UpdateDTO = OdontFlow.Domain.DTOs.Order.UpdateOrderDTO;
using OdontFlow.Application.Services.Contracts;
using OdontFlow.API.Execution.Answers.Contracts;
using OdontFlow.API.Execution;
using System.Net;
using OdontFlow.CrossCutting.Common;
using Microsoft.AspNetCore.SignalR;
using OdontFlow.API.Hubs;
using OdontFlow.Application.Services;
using Azure;
using OdontFlow.Domain.DTOs.Order;

namespace OdontFlow.Presentation.Controllers.Order;

[Produces("application/json")]
[Route("api/v1/[controller]")]
[Authorize]
public class OrderController(IOrderService service, IHubContext<StationWorkHub> _hubContext) : Controller
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get(GetPagedOrdersQuery query)
    {
        async Task<PagedResult<ViewModel>> predicate() => await service.GetAsync(query);
        var response = await SafeExecutor<PagedResult<ViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpGet("{id}/byId")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        async Task<ViewModel> predicate() => await service.GetByIdAsync(id);
        var response = await SafeExecutor<ViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpGet("filter")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetByFilter([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        async Task<IAnswerBase<PagedResult<ViewModel>>> predicate() =>
        Answer<PagedResult<ViewModel>>.SuccessResult(await service.GetByFilterAsync(search, page, pageSize));
        var response = await SafeExecutor<IAnswerBase<PagedResult<ViewModel>>>.ExecAsync(predicate);
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

    [HttpPut("{id}/confirm")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Confirm([FromRoute] Guid id)
    {
        async Task<ViewModel> predicate() => await service.ConfirmOrderAsync(id);
        var response = await SafeExecutor<ViewModel>.ExecAsync(predicate);
        await _hubContext.Clients.All.SendAsync("ReceiveStationWorkUpdate");
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

    [HttpDelete("{id}/deleteOrderItem")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteOrderItem([FromRoute] Guid id)
    {
        async Task<ViewModel> predicate() => await service.DeleteOrdenItemAsync(id);
        var response = await SafeExecutor<ViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpDelete("{id}/deliver")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeliverOrder([FromRoute] Guid id)
    {
        async Task<ViewModel> predicate() => await service.DeliverOrdenAsync(id);
        var response = await SafeExecutor<ViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }



    [AllowAnonymous]
    [HttpGet("{id}/printZirconia")]
    public async Task<IActionResult> PrintZirconia(Guid id)
    {
        async Task<byte[]> predicate() => await service.GenerateRemisionZirconiaPdf(id);
        var pdf = await SafeExecutor<byte[]>.ExecAsync(predicate);
        if (pdf.Success)
        {
            Response.Headers["Content-Disposition"] = "inline; filename=Orden.pdf";
            return File(pdf.Payload, "application/pdf");
        }
        return BadRequest(pdf);
    }

    [AllowAnonymous]
    [HttpGet("{id}/printGamma")]
    public async Task<IActionResult> PrintGamma(Guid id)
    {
        async Task<byte[]> predicate() => await service.GenerateRemisionGammaPdf(id);
        var pdf = await SafeExecutor<byte[]>.ExecAsync(predicate);
        if (pdf.Success)
        {
            Response.Headers["Content-Disposition"] = "inline; filename=Orden.pdf";
            return File(pdf.Payload, "application/pdf");
        }
        return BadRequest(pdf);
    }


    [AllowAnonymous]
    [HttpGet("{id}/printOrder")]
    public async Task<IActionResult> PrintOrder(Guid id)
    {
        async Task<byte[]> predicate() => await service.GeneratePdf(id);
        var pdf = await SafeExecutor<byte[]>.ExecAsync(predicate);
        if (pdf.Success)
        {
            Response.Headers["Content-Disposition"] = "inline; filename=Orden.pdf";
            return File(pdf.Payload, "application/pdf");
        }
        return BadRequest(pdf);
    }

    protected ActionResult ProcessResponse<T>(IAnswerBase<T> response) where T : class
    {
        if (response.Success) return Ok(response);
        else return BadRequest(response);
    }
}
