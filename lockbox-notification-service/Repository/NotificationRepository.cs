using MongoDB.Driver;
using MongoDB.Bson;
using lockbox_notification_service.Models;

namespace lockbox_notification_service.Repository;

public class NotificationRepository : ICrudRepository<NotificationModel>
{
    private MongoClient _client;

    public NotificationRepository()
    {
        var connString = Environment.GetEnvironmentVariable("MONGO_DB_CONN_STRING") ??
                      throw new Exception("Failed to get the MongoDB connection string from environment.");
        
        var settings = MongoClientSettings.FromConnectionString(connString);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);

        _client = new MongoClient(settings);
    }
    
    public void Create(NotificationModel model)
    {
        try
        {
            var database = _client.GetDatabase("Development");
            var notificationCollection = database.GetCollection<BsonDocument>("notifications");
                
            var notificationBson = model.AsBsonDocument();
            notificationCollection.InsertOne(notificationBson);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to store the notification to MongoDB Atlas: {ex.Message}");
        }
    }

    public NotificationModel? Read(string id)
    {
        throw new NotImplementedException();
    }

    public void Update(NotificationModel model)
    {
        throw new NotImplementedException();
    }

    public void Delete(string id)
    {
        throw new NotImplementedException();
    }
}