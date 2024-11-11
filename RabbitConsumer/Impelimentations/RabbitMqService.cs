using Newtonsoft.Json;
using RabbitConsumer.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class RabbitMqService : IRabbitMqService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqService()
    {
        var factory = new ConnectionFactory() { HostName = "localhost", UserName = "Crazy", Password = "2260" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: "UserQueue",
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
        _channel.QueueBind("UserFailedQueue", "user_exchange", "UserFailedQueue");
    }
    public void StartConsuming()
    {
        var unackedMessages = new Dictionary<ulong, (DateTime receivedTime, string queueName)>();
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var user = JsonConvert.DeserializeObject<User>(message);
            Console.WriteLine($"[UserQueue] Received. UserId: {user.Id} - FullName: {user.FullName} - Mail: {user.Email}");
            unackedMessages[ea.DeliveryTag] = (DateTime.UtcNow, "UserQueue");
            try
            {
                _channel.BasicAck(ea.DeliveryTag, false);
                unackedMessages.Remove(ea.DeliveryTag);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };
        _channel.BasicConsume(queue: "UserQueue", autoAck: false, consumer: consumer);
        var failedConsumer = new EventingBasicConsumer(_channel);
        failedConsumer.Received += (model, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var user = JsonConvert.DeserializeObject<User>(message);
            Console.WriteLine($"[UserFailedQueue] Received. UserId: {user.Id} - FullName: {user.FullName} - Mail: {user.Email}");
            unackedMessages[ea.DeliveryTag] = (DateTime.UtcNow, "UserFailedQueue");
            try
            {
                _channel.BasicAck(ea.DeliveryTag, false);
                unackedMessages.Remove(ea.DeliveryTag);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing failed message: {ex.Message}");
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };
        _channel.BasicConsume(queue: "UserFailedQueue", autoAck: false, consumer: failedConsumer);

        Task.Run(() =>
        {
            while (true)
            {
                foreach (var entry in unackedMessages.ToList())
                {
                    var (receivedTime, queueName) = entry.Value;
                    if ((DateTime.UtcNow - receivedTime).TotalSeconds > 30)
                    {
                        Console.WriteLine($"[Timeout] Message in {queueName} not acknowledged, requeuing...");
                        _channel.BasicNack(entry.Key, false, true);
                        unackedMessages.Remove(entry.Key);
                    }
                }
                Thread.Sleep(500);
            }
        });
    }

}

