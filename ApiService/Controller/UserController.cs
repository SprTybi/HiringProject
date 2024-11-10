using Application.Features.Authentication.Query.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiService.Controller
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController( IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            var users = await _mediator.Send(new GetUsersQuery());

            if (users.Code != 0)
                return Unauthorized("Invalid credentials");
            return Ok(new { Token = users.Result });
        }
    }
}