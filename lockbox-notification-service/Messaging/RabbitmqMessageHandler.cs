using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using lockbox_notification_service.Models;

namespace lockbox_notification_service.Messaging;

public class RabbitmqMessageHandler : IMessageHandler
{
    private const string ConnectionString = "Server=127.0.0.1:3306;Database=notification-db;Uid=root;Pwd=password;\n";
    
    public void HandleMessage(string message)
    {
        try
        {
            var msgModel = JsonConvert.DeserializeObject<FileServiceMsgModel>(message);
            if (msgModel == null)
            {
                throw new Exception("The deserialized message is null.");
            }

            var notification = FileMessageToNotification(msgModel);

            using IDbConnection db = new SqlConnection(ConnectionString);
            db.Open();
            var rowsAffected = db.Execute(
                @"INSERT INTO Notifications (Title, Description, UserId) VALUES (@Title, @Description, @UserId)",
                notification);

            if (rowsAffected <= 0)
            {
                throw new Exception("Failed to insert the notification into the database.");
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