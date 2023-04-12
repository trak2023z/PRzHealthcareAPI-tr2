using BoldReports.Writer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Models.DTO;

namespace PRzHealthcareAPI.Controllers
{
    [Route("certificate")]
    [ApiController]
    [Authorize]
    public class CertificateController
    {
        IWebHostEnvironment _hostingEnvironment;

        public CertificateController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }


        
        [HttpPost("printcovidpdf")]
        public IActionResult PrintCOVIDCertificate(string writerFormat)
        {
            // Here, we have loaded the sales-order-detail sample report from application the folder wwwroot\Resources.
            FileStream inputStream = new FileStream(_hostingEnvironment.WebRootPath + @"\Resources\ZaswiadczenieCOVID.rdl", FileMode.Open, FileAccess.Read);
            MemoryStream reportStream = new MemoryStream();
            inputStream.CopyTo(reportStream);
            reportStream.Position = 0;
            inputStream.Close();
            BoldReports.Writer.ReportWriter writer = new BoldReports.Writer.ReportWriter();

            string fileName = null;
            WriterFormat format;
            string type = null;

            if (writerFormat == "PDF")
            {
                fileName = "sales-order-detail.pdf";
                type = "pdf";
                format = WriterFormat.PDF;
            }
            else if (writerFormat == "Word")
            {
                fileName = "sales-order-detail.docx";
                type = "docx";
                format = WriterFormat.Word;
            }
            else if (writerFormat == "CSV")
            {
                fileName = "sales-order-detail.csv";
                type = "csv";
                format = WriterFormat.CSV;
            }
            else
            {
                fileName = "sales-order-detail.xlsx";
                type = "xlsx";
                format = WriterFormat.Excel;
            }

            writer.LoadReport(reportStream);
            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, format);

            // Download the generated export document to the client side.
            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/" + type);
            fileStreamResult.FileDownloadName = fileName;
            return fileStreamResult;
        }
    }
}
