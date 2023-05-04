using BoldReports.Processing.ObjectModels;
using BoldReports.Writer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Helpers;
using PRzHealthcareAPI.Models.DTO;
using PRzHealthcareAPI.Services;
using System.IO;
using System.Net.Mail;
using static Syncfusion.XlsIO.Parser.Biff_Records.TextWithFormat;

namespace PRzHealthcareAPI.Controllers
{
    [Route("certificate")]
    [ApiController]
    [Authorize]
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
        public IActionResult ExportToPDF([FromBody] EventDto dto)
        {
            var score = _certificateService.PrintCOVIDCertificateToPDF(dto);
            return score;
        }
        
    }
}
