using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontFlow.API.Execution;
using OdontFlow.API.Execution.Answers.Contracts;
using OdontFlow.Application.Services.Contracts;
using OdontFlow.Domain.DTOs.Product;
using OdontFlow.Domain.ViewModel.Product;
using System.Net;

namespace OdontFlow.Presentation.Controllers.Auth;

[Produces("application/json")]
[Route("api/v1/[controller]")]
[Authorize]
public class ProductController(IProductsService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get()
    {
        async Task<IEnumerable<ProductViewModel>> predicate() => await service.GetAsync();
        var response = await SafeExecutor<IEnumerable<ProductViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpGet("ByCategory/{id}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetByCategory([FromRoute] PRODUCT_CATEGORY id)
    {
        async Task<IEnumerable<ProductViewModel>> predicate() => await service.GetByCategoryAsync(id);
        var response = await SafeExecutor<IEnumerable<ProductViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpGet("WithoutPlan")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetWithoutPlan()
    {
        async Task<IEnumerable<ProductViewModel>> predicate() => await service.GetWithoutPlanAsync();
        var response = await SafeExecutor<IEnumerable<ProductViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpGet("{id}/byPlanId")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetByPlanId(Guid id)
    {
        async Task<IEnumerable<ProductViewModel>> predicate() => await service.GetByPlanIdAsync(id);
        var response = await SafeExecutor<IEnumerable<ProductViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Post([FromBody] CreateProductDTO input)
    {
        async Task<ProductViewModel> predicate() => await service.CreateAsync(input);
        var response = await SafeExecutor<ProductViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Put([FromBody] UpdateProductDTO input)
    {
        async Task<ProductViewModel> predicate() => await service.UpdateAsync(input);
        var response = await SafeExecutor<ProductViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Delete([FromRoute]Guid id)
    {
        async Task<ProductViewModel> predicate() => await service.DeleteAsync(id);
        var response = await SafeExecutor<ProductViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }



    protected ActionResult ProcessResponse<T>(IAnswerBase<T> response) where T : class
    {
        if (response.Success) return Ok(response);
        else return BadRequest(response);
    }
}
