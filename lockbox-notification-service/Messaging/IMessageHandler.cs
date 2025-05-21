namespace lockbox_notification_service.Messaging;

public interface IMessageHandler
{
    void HandleMessage(string message);
}