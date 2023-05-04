using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Models.DTO;
using PRzHealthcareAPI.Services;
using System.Security.Claims;

namespace PRzHealthcareAPI.Controllers
{
    [Route("notification")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult GetAllTypes()
        {
            var types = _notificationService.GetAllTypes();
            return Ok(types);
        }

        [HttpPut("editnotificationtype")]
        public ActionResult EditNotificationType([FromBody] NotificationTypeDto dto)
        {
            int accountId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                accountId = Convert.ToInt32(identity.FindFirst(ClaimTypes.SerialNumber).Value);
            }
            _notificationService.EditNotificationType(dto, accountId);
            return Ok();
        }
    }
}
