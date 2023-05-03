using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Models.DTO;
using PRzHealthcareAPI.Services;
using System.Security.Claims;

namespace PRzHealthcareAPI.Controllers
{
    [Route("vaccination")]
    [ApiController]
    [Authorize]
    public class VaccinationController : ControllerBase
    {
        private readonly IVaccinationService _vaccinationService;

        public VaccinationController(IVaccinationService vaccinationService)
        {
            this._vaccinationService = vaccinationService;
        }

        [HttpGet("getall")]
        public ActionResult GetVaccinationList()
        {
            var vaccinationDtos = _vaccinationService.GetAll();
            return Ok(vaccinationDtos);
        }
        [HttpPut("addvaccination")]
        public ActionResult AddVaccination([FromBody] VaccinationDto dto)
        {
            int accountId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                accountId = Convert.ToInt32(identity.FindFirst(ClaimTypes.SerialNumber).Value);
            }
            _vaccinationService.AddVaccination(dto, accountId);
            return Ok();
        }
        [HttpPut("editvaccination")]
        public ActionResult EditVaccination([FromBody] VaccinationDto dto)
        {
            int accountId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                accountId = Convert.ToInt32(identity.FindFirst(ClaimTypes.SerialNumber).Value);
            }
            _vaccinationService.EditVaccination(dto, accountId);
            return Ok();
        }
        [HttpPut("archivevaccination")]
        public ActionResult ArchiveVaccination([FromBody] VaccinationDto dto)
        {
            int accountId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                accountId = Convert.ToInt32(identity.FindFirst(ClaimTypes.SerialNumber).Value);
            }
            _vaccinationService.ArchiveVaccination(dto, accountId);
            return Ok();
        }
        [HttpPut("unarchivevaccination")]
        public ActionResult UnarchiveVaccination([FromBody] VaccinationDto dto)
        {
            int accountId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                accountId = Convert.ToInt32(identity.FindFirst(ClaimTypes.SerialNumber).Value);
            }
            _vaccinationService.UnarchiveVaccination(dto, accountId);
            return Ok();
        }
    }
}
