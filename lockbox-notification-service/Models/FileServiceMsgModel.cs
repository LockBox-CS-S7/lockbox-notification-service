namespace lockbox_notification_service.Models;

public class FileServiceMsgModel
{
    public string EventType { get; set; }
    public string TimeStamp { get; set; }
    public string Source { get; set; }
    public string UserId { get; set; }
    public string? File { get; set; }
}