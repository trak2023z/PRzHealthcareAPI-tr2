using BoldReports.Writer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PRzHealthcareAPI.Exceptions;
using PRzHealthcareAPI.Helpers;
using PRzHealthcareAPI.Models;
using PRzHealthcareAPI.Models.DTO;
using System.IO;

namespace PRzHealthcareAPI.Services
{
    public interface ICertificateService
    {
        FileStreamResult PrintCOVIDCertificateToPDF(EventDto dto);
        MemoryStream PrintCOVIDCertificateToMemoryStream(EventDto dto);
        void AddCertificate(string certificateFilePath, int accountId);
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
        /// Dodanie wydruku certyfikatu do bazy danych
        /// </summary>
        /// <param name="certificateFilePath">Ścieżka do certyfikatu</param>
        /// <param name="accountId">Id zalogowanego użytkownika</param>
        public void AddCertificate(string certificateFilePath, int accountId)
        {
            try
            {
                if (!_dbContext.BinData.Any(x => x.Bin_Name == "Certyfikat szczepienia"))
                {
                    var certificateContent = Tools.ToBase64Converter(certificateFilePath);
                    BinData data = new BinData()
                    {
                        Bin_Name = "Certyfikat szczepienia",
                        Bin_Data = certificateContent,
                        Bin_InsertedDate = DateTime.Now,
                        Bin_ModifiedDate = DateTime.Now,
                        Bin_InsertedAccId = accountId,
                        Bin_ModifiedAccId = accountId,
                    };

                    _dbContext.BinData.Add(data);
                    _dbContext.SaveChanges();

                    Certificate certificate = new Certificate()
                    {
                        Cer_AccId = accountId,
                        Cer_BinId = 1,
                        Cer_IsActive = true,
                        Cer_InsertedDate = DateTime.Now,
                        Cer_InsertedAccId = accountId,
                        Cer_ModifiedAccId = accountId,
                        Cer_ModifiedDate = DateTime.Now
                    };

                    _dbContext.Certificates.Add(certificate);
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
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
                var certificate = _dbContext.Certificates.FirstOrDefault(x => x.Cer_Name == "Certyfikat szczepienia COVID");
                var baseCode = _dbContext.BinData.FirstOrDefault(x => x.Bin_Id == certificate.Cer_BinId).Bin_Data;
                var filePath = Tools.FromBase64Converter(_hostingEnvironment, baseCode);
                FileStream reportStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
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
                reportStream.Dispose();
                File.Delete(filePath);
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
                var certificate = _dbContext.Certificates.FirstOrDefault(x => x.Cer_Name == "Certyfikat szczepienia COVID");
                var baseCode = _dbContext.BinData.FirstOrDefault(x => x.Bin_Id == certificate.Cer_BinId).Bin_Data;
                var filePath = Tools.FromBase64Converter(_hostingEnvironment, baseCode);
                FileStream reportStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
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
