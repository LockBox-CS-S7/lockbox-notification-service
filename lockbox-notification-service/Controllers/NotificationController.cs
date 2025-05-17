using lockbox_notification_service.Models;
using Microsoft.AspNetCore.Mvc;

namespace lockbox_notification_service.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    private readonly ILogger<NotificationController> _logger;
    
    private readonly List<NotificationModel> _mockModels =
    [
        new NotificationModel(
            "yn9w3myt9cw3y",
            "test notification",
            "This is a test notification",
            "926yn983y93yt9"
        ),

        new NotificationModel(
            "hosmohjh",
            "wow",
            "ow wow",
            "noob"
        ),

        new NotificationModel(
            "mwmy9hc_hw9h",
            "file uploaded",
            "Successfully uploaded a file!",
            "noob"
        )
    ];

    public NotificationController(ILogger<NotificationController> logger)
    {
        _logger = logger;
    }

    [HttpGet("user-notifications/{userId}")]
    public ActionResult<NotificationModel> GetNotificationsByUserId(string userId)
    {
        _logger.LogInformation("Someone requested all notifications from the user with id: {userId}", userId);

        foreach (var notification in _mockModels)
        {
            if (notification.UserId == userId)
            {
                return Ok(notification);
            }
        }

        return NotFound("No notification with the given id was found");
    }
}