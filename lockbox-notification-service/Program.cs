using DotNetEnv;
using lockbox_notification_service.Messaging;

namespace lockbox_notification_service;

public class Program
{
    public static void Main(string[] args)
    {
        Env.Load();
        
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        
        // Add the RabbitMQ background service
        builder.Services.AddHostedService<RabbitMqBackgroundService>(sp => 
            new RabbitMqBackgroundService(
                queueName: "file-queue",
                logger: sp.GetRequiredService<ILogger<RabbitMqBackgroundService>>()
            )
        );
        
        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        // Start collecting metrics for Prometheus
        using var server = new Prometheus.MetricServer(port: 1234);
        server.Start();
        
        app.Run();
    }
}