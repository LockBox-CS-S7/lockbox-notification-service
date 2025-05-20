namespace lockbox_notification_service.Messaging;

public class RabbitmqMessageHandler : IMessageHandler
{
    public void HandleMessage(string message)
    {
        // TODO: convert the message to a "FileServiceMsgModel" from JSON, then store in database using dapper.
    }
}