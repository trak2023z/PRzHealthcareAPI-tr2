using BoldReports.Writer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Models.DTO;
using PRzHealthcareAPI.Services;
using static Syncfusion.XlsIO.Parser.Biff_Records.TextWithFormat;

namespace PRzHealthcareAPI.Controllers
{
    [Route("certificate")]
    [ApiController]
    //[Authorize]
    public class CertificateController
    {
        private readonly ICertificateService _certificateService;
        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;

        public CertificateController(ICertificateService certificateService, Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
        {
            _certificateService = certificateService;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("covid")]
        public IActionResult Export() //[FromBody] EventDto? dto
        {
            EventDto dto = new EventDto();
            var score = _certificateService.PrintCOVIDCertificateToPDF(dto);
            return score;
        }
    }
}
