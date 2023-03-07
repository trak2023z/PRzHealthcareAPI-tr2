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
            _eventService.SeedOffDates();
            return Ok();
        }

        [HttpGet("getavailabledates")]
        public ActionResult GetAvailableDates(int doctorId)
        {
            var availableEvents = _eventService.GetAvailableDates(doctorId);
            return Ok(availableEvents);
        }
     }
}
