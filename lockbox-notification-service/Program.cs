namespace lockbox_notification_service;

public class Program
{
    public static void Main(string[] args)
    {
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
        
        app.Run();
    }
}