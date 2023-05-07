using BoldReports.Writer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PRzHealthcareAPI.Exceptions;
using PRzHealthcareAPI.Models;
using PRzHealthcareAPI.Models.DTO;
using System.IO;

namespace PRzHealthcareAPI.Services
{
    public interface ICertificateService
    {
        FileStreamResult PrintCOVIDCertificateToPDF(EventDto dto);
        MemoryStream PrintCOVIDCertificateToMemoryStream(EventDto dto);
    }

    public class CertificateService : ICertificateService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly HealthcareDbContext _dbContext;

        public CertificateService(IWebHostEnvironment hostingEnvironment, HealthcareDbContext dbContext)
        {
            _hostingEnvironment = hostingEnvironment;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Wydruk zaświadczenia COVID do pliku PDF
        /// </summary>
        /// <param name="dto">Model eventu</param>
        /// <returns>Plik z wygenerowaniem wydrukiem</returns>
        /// <exception cref="BadRequestException">Błąd podczas próby wydruku</exception>
        public FileStreamResult PrintCOVIDCertificateToPDF(EventDto dto)
        {
            try
            {
                if (dto is null)
                {
                    dto = new EventDto() { Id = 7195 };
                }
                var contentRootPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Resources");
                DirectoryInfo contentDirectoryInfo = new DirectoryInfo(contentRootPath);
                var resoruceFilesInfo = contentDirectoryInfo.GetFiles("*.rdl", SearchOption.AllDirectories);
                var resoruceFileNames = resoruceFilesInfo.Select(info => info.FullName);


                FileStream reportStream = new FileStream(resoruceFileNames.FirstOrDefault(), FileMode.Open, FileAccess.Read);
                BoldReports.Writer.ReportWriter writer = new BoldReports.Writer.ReportWriter();
                writer.ReportProcessingMode = ProcessingMode.Remote;
                List<BoldReports.Web.ReportParameter> userParameters = new List<BoldReports.Web.ReportParameter>
            {
                new BoldReports.Web.ReportParameter()
                {
                    Name = "EventId",
                    Values = new List<string>() { dto.Id.ToString() }
                }
            };



                string fileName = "ZaswiadczenieCOVID.pdf";
                string type = "pdf";
                WriterFormat format = WriterFormat.PDF;

                writer.LoadReport(reportStream);
                writer.SetParameters(userParameters);
                MemoryStream memoryStream = new MemoryStream();
                writer.Save(memoryStream, format);

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

        /// <summary>
        /// Wydruk zaświadczenia COVID do pamięci systemu
        /// </summary>
        /// <param name="dto">Model eventu</param>
        /// <returns>MemoryStream z wygenerowaniem wydrukiem</returns>
        /// <exception cref="BadRequestException">Błąd podczas próby wydruku</exception>
        public MemoryStream PrintCOVIDCertificateToMemoryStream(EventDto dto)
        {
            try
            {
                if (dto is null)
                {
                    dto = new EventDto() { Id = 7195 };
                }
                var contentRootPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Resources");
                DirectoryInfo contentDirectoryInfo = new DirectoryInfo(contentRootPath);
                var resoruceFilesInfo = contentDirectoryInfo.GetFiles("*.rdl", SearchOption.AllDirectories);
                var resoruceFileNames = resoruceFilesInfo.Select(info => info.FullName);


                FileStream reportStream = new FileStream(resoruceFileNames.FirstOrDefault(), FileMode.Open, FileAccess.Read);
                BoldReports.Writer.ReportWriter writer = new BoldReports.Writer.ReportWriter();
                writer.ReportProcessingMode = ProcessingMode.Remote;
                List<BoldReports.Web.ReportParameter> userParameters = new List<BoldReports.Web.ReportParameter>
            {
                new BoldReports.Web.ReportParameter()
                {
                    Name = "EventId",
                    Values = new List<string>() { dto.Id.ToString() }
                }
            };

                WriterFormat format = WriterFormat.PDF;

                writer.LoadReport(reportStream);
                writer.SetParameters(userParameters);
                MemoryStream memoryStream = new MemoryStream();
                writer.Save(memoryStream, format);

                string fileName = Path.Combine(Path.GetTempPath(), $@"certificate{dto.Id}.pdf");
                using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                {
                    memoryStream.WriteTo(fileStream);
                    fileStream.Dispose();
                }

                return memoryStream;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
    }
}
