using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace lockbox_notification_service.Messaging
{
    public class RabbitMqBackgroundService : BackgroundService
    {
        private readonly string _queueName;
        private readonly ILogger<RabbitMqBackgroundService> _logger;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly ConnectionFactory _factory;
        private readonly IMessageHandler _messageHandler;
        
        public RabbitMqBackgroundService(string queueName, ILogger<RabbitMqBackgroundService> logger)
        {
            _queueName = queueName;
            _logger = logger;

            string rabbitUri = Environment.GetEnvironmentVariable("RABBITMQ_CONN_STRING") ??
                               throw new Exception("Could not get the RabbitMQ broker uri from environment.");
            
            _factory = new ConnectionFactory
            {
                Uri = new Uri(rabbitUri)
            };
            
            _messageHandler = new RabbitmqMessageHandler();
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
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
            
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null) throw new InvalidOperationException("Channel has not been initialized.");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (models, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Received message: {message}", message);
                
                await _messageHandler.HandleMessage(message);
            };
            
            await _channel.BasicConsumeAsync(_queueName, autoAck: true, consumer: consumer, cancellationToken: stoppingToken);
            _logger.LogInformation("Started consuming queue {Queue}", _queueName);

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // This runs when the cancellation token tells to stop the process.
                _logger.LogError("The RabbitMqBackgroundService caught a \"TaskCanceledException\"");
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