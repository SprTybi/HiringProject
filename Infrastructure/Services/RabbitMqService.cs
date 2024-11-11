using Application.Common.Interfaces.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Application.Common.Models;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

public class RabbitMqService : IRabbitMqService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqService(IConfiguration configuration)
    {
        var connection = new ConnectionFactory()
        {
            HostName = configuration["RabbitMQ:Host"],
            UserName = configuration["RabbitMQ:Username"],
            Password = configuration["RabbitMQ:Password"]
        };
        _connection = connection.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare("user_exchange", ExchangeType.Direct);

        _channel.QueueDeclare(queue: "UserQueue",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: new Dictionary<string, object>
                             {
                                 { "x-dead-letter-exchange", "user_exchange" },
                                 { "x-dead-letter-routing-key", "UserFailedQueue" },
                                 { "x-message-ttl", 10000 }
                             });
        _channel.QueueDeclare(queue: "UserFailedQueue", durable: false, exclusive: false, autoDelete: false);
        _channel.QueueBind("UserQueue", "user_exchange", "UserQueue"); 
        _channel.QueueBind("UserFailedQueue", "user_exchange", "UserFailedQueue");

    }

    public async Task SendMessageAsync(object obj)
    {
        try
        {
            var message = JsonConvert.SerializeObject(obj);
            var body = Encoding.UTF8.GetBytes(message); 

            _channel.BasicPublish(
                exchange: "",
                routingKey: "UserQueue", 
                basicProperties: null,
                body: body);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
            await Task.CompletedTask;

        }
    }

}
