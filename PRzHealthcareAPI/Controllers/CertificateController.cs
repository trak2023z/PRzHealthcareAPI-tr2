using BoldReports.Writer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Models.DTO;
using PRzHealthcareAPI.Services;

namespace PRzHealthcareAPI.Controllers
{
    [Route("certificate")]
    [ApiController]
    [Authorize]
    public class CertificateController
    {
        private readonly ICertificateService _certificateService;

        public CertificateController(ICertificateService certificateService)
        {
            this._certificateService = certificateService;
        }


        
        [HttpPost("printcovidpdf")]
        public IActionResult PrintCOVIDCertificate()
        {
            var print = _certificateService.PrintCOVIDCertificate();
            return print;
        }
    }
}
