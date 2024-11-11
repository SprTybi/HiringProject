using Application.Common.Models;
using MediatR;

namespace Application.Features.RabbitMQ.Command
{
    public class SendUserToQueueCommand : IRequest<Result_VM<bool>>
    {
        public int UserId { get; set; }
    }
}
