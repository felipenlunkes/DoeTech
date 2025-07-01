using DoeTech.DTOs;
using DoeTech.Models;
using DoeTech.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoeTech.Controllers;

[Route("rest/v{version:apiVersion}/notification")]
[ApiVersion("1")]
public class NotificationController(NotificationService notificationService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public IActionResult Add([FromBody] Notification request)
    {
        if (request == null)
        {
            return BadRequest("The request body could not be null.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }


        var notification = notificationService.Add(request);

        return Ok(notification);
    }

    [Authorize]
    [HttpPut("{notificationId}")]
    public IActionResult Update(Guid notificationId, [FromBody] Notification request)
    {
        
        if (request == null)
        {
            return BadRequest("The request body could not be null.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var updatedNotification = notificationService.Update(notificationId, request);
        
        return Ok(updatedNotification);
    }
    
    [Authorize]
    [HttpPost("{notificationId}")]
    public IActionResult MarkAsReadById(Guid notificationId)
    {
        
        notificationService.MarkAsRead(notificationId);
        
        return Ok();
    }
    
    [Authorize]
    [HttpGet("{notificationId}")]
    public IActionResult GetById(Guid notificationId)
    {
        var notification = notificationService.GetById(notificationId);

        return Ok(notification);
    }
    
    [Authorize]
    [HttpGet("{accountId}/all")]
    public IActionResult GetAllByAccountId(Guid accountId)
    {
        var notifications = notificationService.GetAllByAccountId(accountId);

        return Ok(notifications);
    }
    
    [Authorize]
    [HttpGet("query")]
    public IActionResult Query([FromQuery] NotificationDTO filters)
    {
        var notifications = notificationService.Query(filters);

        return Ok(notifications);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete]
    public IActionResult Remove(Guid notificationId)
    {

        notificationService.RemoveMessage(notificationId);

        return NoContent();
    }
}