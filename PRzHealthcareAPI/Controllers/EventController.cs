﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Helpers;
using PRzHealthcareAPI.Models.DTO;
using PRzHealthcareAPI.Services;
using System.Security.Claims;

namespace PRzHealthcareAPI.Controllers
{
    [Route("event")]
    [ApiController]
    [Authorize]

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

        [HttpGet("getdoctorevents")]
        public ActionResult GetDoctorEvents([FromQuery] int accountId)
        {
            var availableEvents = _eventService.GetDoctorEvents(accountId);
            return Ok(availableEvents);
        }

        [HttpGet("getnurseevents")]
        public ActionResult GetNurseEvents()
        {
            var availableEvents = _eventService.GetNurseEvents();
            return Ok(availableEvents);
        }

        [HttpPost("newevent")]
        public ActionResult TakeTerm(EventDto dto)
        {
            string accountId = "";
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                accountId = identity.FindFirst(ClaimTypes.SerialNumber).Value;
            }

            _eventService.TakeTerm(dto, accountId);
            return Ok();
        }

        [HttpPatch("printcertificate")]
        public ActionResult PrintCertificate()
        {
            BoldReportsApi.PrintPDF();

            return Ok();
        }
    }
}
