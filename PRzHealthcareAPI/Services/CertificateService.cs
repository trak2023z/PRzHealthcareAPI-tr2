using BoldReports.Writer;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Exceptions;

namespace PRzHealthcareAPI.Services
{
    public interface ICertificateService
    {
        ActionResult PrintCOVIDCertificate();
    }

    public class CertificateService : ICertificateService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CertificateService(IWebHostEnvironment hostingEnvironment)
        {
            this._hostingEnvironment = hostingEnvironment;
        }

        public ActionResult PrintCOVIDCertificate()
        {
            try
            {
                FileStream inputStream = new FileStream(_hostingEnvironment.WebRootPath + @"\Resources\ZaswiadczenieCOVID.rdl", FileMode.Open, FileAccess.Read);
                MemoryStream reportStream = new MemoryStream();
                inputStream.CopyTo(reportStream);
                reportStream.Position = 0;
                inputStream.Close();
                BoldReports.Writer.ReportWriter writer = new BoldReports.Writer.ReportWriter();

                string fileName = null;
                WriterFormat format;
                string type = null;

                fileName = "ZaswiadczenieCOVID.pdf";
                type = "pdf";
                format = WriterFormat.PDF;

                writer.LoadReport(reportStream);
                MemoryStream memoryStream = new MemoryStream();
                writer.Save(memoryStream, format);

                // Download the generated export document to the client side.
                memoryStream.Position = 0;
                FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/" + type);
                fileStreamResult.FileDownloadName = fileName;
                return fileStreamResult;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
    }
}
