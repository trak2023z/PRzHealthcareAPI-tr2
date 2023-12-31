﻿using BoldReports.Processing.ObjectModels;
using BoldReports.Writer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Helpers;
using PRzHealthcareAPI.Models.DTO;
using PRzHealthcareAPI.Services;
using System.IO;
using System.Net.Mail;
using System.Security.Claims;
using static Syncfusion.XlsIO.Parser.Biff_Records.TextWithFormat;

namespace PRzHealthcareAPI.Controllers
{
    [Route("certificate")]
    [ApiController]
    [Authorize]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificateService;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;

        public CertificateController(ICertificateService certificateService, IWebHostEnvironment hostingEnvironment)
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

        [HttpPost("addcovidcertificate")]
        public ActionResult AddCOVIDCertificate([FromQuery] string certificateFilePath)
        {
            int accountId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                accountId = Convert.ToInt32(identity.FindFirst(ClaimTypes.SerialNumber).Value);
            }
            _certificateService.AddCertificate(certificateFilePath, accountId);
            return Ok();
        }

    }
}
