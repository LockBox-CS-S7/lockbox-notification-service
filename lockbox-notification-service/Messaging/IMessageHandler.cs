namespace lockbox_notification_service.Messaging;

public interface IMessageHandler
{
    Task HandleMessage(string message);
}