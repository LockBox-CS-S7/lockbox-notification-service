using MongoDB.Bson;

namespace lockbox_notification_service.Models;

public class NotificationModel
{
    public string? Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string UserId { get; set; }

    public NotificationModel(string? id, string title, string description, string userId)
    {
        Id = id;
        Title = title;
        Description = description;
        UserId = userId;
    }

    public BsonDocument AsBsonDocument()
    {
        return new BsonDocument
        {
            { "id", Id },
            { "title", Title},
            { "description", Description},
            { "user_id", UserId }
        };
    }
}