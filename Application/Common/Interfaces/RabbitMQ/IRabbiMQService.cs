using Application.Common.Models;

namespace Application.Common.Interfaces.RabbitMQ
{
    public interface IRabbitMqService
    {
        Task SendMessageAsync(object obj);
    }
}
