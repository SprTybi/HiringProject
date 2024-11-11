using Application.Features.RabbitMQ.Command;
using Application.Features.Users.Query.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiService.Controller
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _mediator.Send(new GetUsersQuery());

            if (users.Code != 0)
                return Unauthorized("Invalid credentials");
            return Ok(new { Token = users.Result });
        }

        [HttpPost("sendToQueue")]
        public async Task<IActionResult> SendToQueue()
        {
            var sendToQueue = await _mediator.Send(new SendUserToQueueCommand { UserId = 1 });
            return Ok("Message sent to queue");
        }
    }

}