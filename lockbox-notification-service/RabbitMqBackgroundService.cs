using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace lockbox_notification_service
{
    public class RabbitMqBackgroundService : BackgroundService
    {
        private readonly string _queueName;
        private readonly ILogger<RabbitMqBackgroundService> _logger;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly ConnectionFactory _factory;

        public RabbitMqBackgroundService(string queueName, ILogger<RabbitMqBackgroundService> logger)
        {
            _queueName = queueName;
            _logger = logger;
            _factory = new ConnectionFactory { HostName = "localhost" }; 
        }

        public override async Task<Task> StartAsync(CancellationToken cancellationToken)
        {
            _connection = await _factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null, 
                cancellationToken: cancellationToken);
            
            _logger.LogInformation("RabbitMQ connection/channel established and queue declared: {Queue}", _queueName);
            
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null) throw new InvalidOperationException("Channel has not been initialized.");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (Models, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Received message: {message}", message);
                
                // TODO: Do something with the message here...
            };
            
            await _channel.BasicConsumeAsync(_queueName, autoAck: true, consumer: consumer);
            _logger.LogInformation("Started consuming queue {Queue}", _queueName);

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // This runs when the cancellation token tells to stop the process.
                _logger.LogError("Caught a \"TaskCanceledException\"");
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Shutting down RabbitMQ consumer for queue {Queue}", _queueName);

            if (_channel is { IsOpen: true }) _channel.CloseAsync(cancellationToken);
            if (_connection is { IsOpen: true }) _connection.CloseAsync(cancellationToken);
            
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}