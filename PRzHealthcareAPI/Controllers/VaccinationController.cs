using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Services;

namespace PRzHealthcareAPI.Controllers
{
    [Route("vaccination")]
    [ApiController]
    public class VaccinationController : ControllerBase
    {
        private readonly IVaccinationService _vaccinationService;

        public VaccinationController(IVaccinationService vaccinationService)
        {
            this._vaccinationService = vaccinationService;
        }

        [HttpGet("getallactive")]
        //todo: [Authorize]
        public ActionResult GetVaccinationList()
        {
            var vaccinationDtos = _vaccinationService.GetAllActive();
            return Ok(vaccinationDtos);
        }
    }
}
