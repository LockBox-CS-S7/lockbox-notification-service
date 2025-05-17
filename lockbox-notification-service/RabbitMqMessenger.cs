using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace lockbox_notification_service;

public class RabbitMqMessenger
{
    private readonly string _queueName;

    public RabbitMqMessenger(string queueName)
    {
        _queueName = queueName;
    }

    public async Task StartConnection()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false,
            arguments: null);
        
        Console.WriteLine(" [*] Waiting for messages.");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [*] Received {message}");
            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(_queueName, autoAck: true, consumer: consumer);
        
        Console.WriteLine(" Press [Enter] to exit.");
        Console.ReadLine();
    }
}