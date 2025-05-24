using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using lockbox_notification_service.Models;


namespace lockbox_notification_service.Messaging;

public class RabbitmqMessageHandler : IMessageHandler
{
    private string _mongoConnString;

    public RabbitmqMessageHandler()
    {
        _mongoConnString = Environment.GetEnvironmentVariable("MONGO_DB_CONN_STRING") ??
                           throw new Exception("Failed to get the MongoDB connection string from environment.");
    }
    
    public void HandleMessage(string message)
    {
        try
        {
            var msgModel = JsonConvert.DeserializeObject<FileServiceMsgModel>(message) ??
                           throw new Exception("The deserialized message is null.");

            var notification = FileMessageToNotification(msgModel);

            try
            {
                var mongoConnString = Environment.GetEnvironmentVariable("MONGO_DB_CONN_STRING");
                var settings = MongoClientSettings.FromConnectionString(mongoConnString);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);

                var client = new MongoClient(settings);
                var database = client.GetDatabase("Development");
                var notificationCollection = database.GetCollection<NotificationModel>("notifications");
                
                notificationCollection.InsertOne(notification);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to store the notification to MongoDB Atlas: {ex.Message}");
            }
        }
        catch (JsonSerializationException ex)
        {
            Console.WriteLine($"Failed to deserialize incoming message: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Failed to connect to database: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    /// <summary>
    /// Converts a message from the FileStorageService into a usable notification model.
    /// </summary>
    /// <param name="model">The model generated for the incoming message from the FileStorageService</param>
    /// <returns>A NotificationModel containing the information needed for a user notification.</returns>
    private NotificationModel FileMessageToNotification(FileServiceMsgModel model)
    {
        // TODO: More logic should be implemented here, it will not always be that a file was uploaded.
        // The model.EventType should be checked for this.
        return new NotificationModel(
            null,
            "File upload successfull",
            $"You successfully uploaded a file: {model.File}",
            model.UserId
        );
    }
}