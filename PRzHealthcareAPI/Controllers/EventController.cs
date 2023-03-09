using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Models.DTO;
using PRzHealthcareAPI.Services;

namespace PRzHealthcareAPI.Controllers
{
    [Route("event")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            this._eventService = eventService;
        }

        [HttpPost("seed")]
        public async Task<IActionResult> Seed()
        {
            _eventService.SeedDates();
            return Ok();
        }

        [HttpGet("getavailabledates")]
        public ActionResult GetAvailableDates([FromQuery] string selectedDate, [FromQuery] string selectedDoctorId)
        {
            var availableEvents = _eventService.GetAvailableDates(selectedDate, selectedDoctorId);
            return Ok(availableEvents);
        }

        [HttpGet("getuserevents")]
        public ActionResult GetUserEvents([FromQuery] int accountId)
        {
            var availableEvents = _eventService.GetUserEvents(accountId);
            return Ok(availableEvents);
        }
    }
}
