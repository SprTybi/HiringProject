class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Consuming messages with multiple consumers...");

        var rabbitMqService = new RabbitMqService();
        rabbitMqService.StartConsuming();
        Console.ReadLine();
    }
}

