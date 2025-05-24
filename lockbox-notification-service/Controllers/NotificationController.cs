using lockbox_notification_service.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace lockbox_notification_service.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    private readonly ILogger<NotificationController> _logger;
    private readonly MongoClient _mongoClient;
    

    public NotificationController(ILogger<NotificationController> logger)
    {
        _logger = logger;
        
        var mongoConnString = Environment.GetEnvironmentVariable("MONGO_DB_CONN_STRING") ?? 
                              throw new Exception("Failed to get the MongoDB connection string from environment.");
        var settings = MongoClientSettings.FromConnectionString(mongoConnString);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        
        _mongoClient = new MongoClient(settings);
    }

    [HttpGet("/")]
    public async Task<ActionResult<List<NotificationModel>>> GetAllNotifications()
    {
        _logger.LogInformation("Someone requested all notifications.");
        
        var database = _mongoClient.GetDatabase("Development");
        var notificationCollection = database.GetCollection<NotificationModel>("notifications");

        var notifications = await notificationCollection.Find(new BsonDocument()).ToListAsync();
        
        return Ok(notifications);
    }

    [HttpGet("user-notifications/{userId}")]
    public async Task<ActionResult<List<NotificationModel>>>  GetNotificationsByUserId(string userId)
    {
        _logger.LogInformation("Someone requested all notifications from the user with id: {userId}", userId);

        var database = _mongoClient.GetDatabase("Development");
        var notificationCollection = database.GetCollection<NotificationModel>("notifications");

        var filter = Builders<NotificationModel>.Filter.Eq(doc => doc.UserId, userId);
        var foundNotifications = await notificationCollection.Find(filter).ToListAsync();

        if (foundNotifications == null || foundNotifications.Count == 0)
        {
            return NotFound("No notification with the given id was found");
        }

        return Ok(foundNotifications);
    }
}