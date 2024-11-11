using Application.Common.Interfaces.RabbitMQ;
using Application.Common.Interfaces.Users;
using Application.Common.Models;
using MediatR;

namespace Application.Features.RabbitMQ.Command
{
    public class SendUserToQueueCommandHandler :
        IRequestHandler<SendUserToQueueCommand,
        Result_VM<bool>>
    {
        private readonly IRabbitMqService _rabbitMqService;
        private readonly IUserRepository _userRepository;

        public SendUserToQueueCommandHandler(IRabbitMqService rabbitMqService, IUserRepository userRepository)
        {
            _rabbitMqService = rabbitMqService;
            _userRepository = userRepository;
        }

        public async Task<Result_VM<bool>> Handle(SendUserToQueueCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetUserById(request.UserId);
                if (user.Result == null)
                {
                    return new()
                    {
                        Code = user.Code,
                        Message = user.Message
                    };
                }

                await _rabbitMqService.SendMessageAsync(user.Result);
                return new Result_VM<bool>
                {
                    Code = 0,
                    Message = "Message sent to queue successfully."
                };
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                return new Result_VM<bool>
                {
                    Code = -1,
                    Message = "Error sending message"
                };
            }
        }
    }

}
