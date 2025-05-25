using Newtonsoft.Json;

namespace lockbox_notification_service.Models;

public class FileServiceMsgModel
{
    [JsonProperty("event_type")]
    public string EventType { get; set; }
    
    [JsonProperty("timestamp")]
    public string TimeStamp { get; set; }
    
    [JsonProperty("source")]
    public string Source { get; set; }
    
    [JsonProperty("user_id")]
    public string UserId { get; set; }
    
    [JsonProperty("file")]
    public string? File { get; set; }

    public FileServiceMsgModel(string eventType, string timeStamp, string source, string userId, string? file)
    {
        EventType = eventType;
        TimeStamp = timeStamp;
        Source = source;
        UserId = userId;
        File = file;
    }
}