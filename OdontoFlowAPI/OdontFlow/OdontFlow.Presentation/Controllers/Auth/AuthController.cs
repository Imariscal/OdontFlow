using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontFlow.API.Execution;
using OdontFlow.API.Execution.Answers.Contracts;
using OdontFlow.Application.Services.Contracts;
using OdontFlow.Domain.DTOs;
using OdontFlow.Domain.DTOs.User;
using OdontFlow.Domain.ViewModel.User;
using System.Net;
using System.Security.Claims;

namespace OdontFlow.Presentation.Controllers.Auth;

[Produces("application/json")]
[Route("api/v1/[controller]")]
[AllowAnonymous]
public class AuthController(IAuthService service) : Controller
{
    [HttpPost]
    [AllowAnonymous]
    [Route("Login")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest login)
    {
        async Task<AuthResult> predicate() => await service.LoginAsync(login);
        var response = await SafeExecutor<AuthResult>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("RegisterLogin")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterLogin([FromBody] RegisterRequest login)
    {
        async Task<AuthResult> predicate() => await service.RegisterAsync(login);
        var response = await SafeExecutor<AuthResult>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPost]
    [Authorize]
    [HttpPost("ChangePassword")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> ChangePassword([FromBody] ResetFirstPasswordRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  
        if (!Guid.TryParse(userId, out var guid))
            return Unauthorized();

        async Task<AuthResult> predicate() => await service.ChangeInitialPassword(guid, request);
        var response = await SafeExecutor<AuthResult>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpPut] 
    [Route("UpdateLogin")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateLogin([FromBody] UpdateUserDTO login)
    {
        async Task<UserViewModel> predicate() => await service.UpdateAsync(login);
        var response = await SafeExecutor<UserViewModel>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    [HttpGet] 
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get()
    {
        async Task<IEnumerable<UserViewModel>> predicate() => await service.GetUsersAsync();
            var response = await SafeExecutor<IEnumerable<UserViewModel>>.ExecAsync(predicate);
        return ProcessResponse(response);
    }

    protected ActionResult ProcessResponse<T>(IAnswerBase<T> response) where T : class
    {
        if (response.Success) return Ok(response);
        else return BadRequest(response);
    }
}
