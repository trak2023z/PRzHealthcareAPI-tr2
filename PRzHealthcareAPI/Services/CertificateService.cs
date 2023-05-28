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
                    BinData data = new()
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

                    Certificate certificate = new()
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
                byte[] fileBytes = Convert.FromBase64String(baseCode);

                using MemoryStream stream = new(fileBytes);
                ReportWriter writer = new()
                {
                    ReportProcessingMode = ProcessingMode.Remote
                };
                List<BoldReports.Web.ReportParameter> userParameters = new() { new BoldReports.Web.ReportParameter() { Name = "EventId", Values = new List<string>() { dto.Id.ToString() } } };



                string fileName = "ZaswiadczenieCOVID.pdf";
                string type = "pdf";
                WriterFormat format = WriterFormat.PDF;

                writer.LoadReport(stream);
                writer.SetParameters(userParameters);

                MemoryStream memoryStream = new();
                writer.Save(memoryStream, format);
                memoryStream.Position = 0;

                FileStreamResult fileStreamResult = new(memoryStream, "application/" + type)
                {
                    FileDownloadName = fileName
                };

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
                var certificate = _dbContext.Certificates.FirstOrDefault(x => x.Cer_Name == "Certyfikat szczepienia COVID");
                var baseCode = _dbContext.BinData.FirstOrDefault(x => x.Bin_Id == certificate.Cer_BinId).Bin_Data;
                byte[] fileBytes = Convert.FromBase64String(baseCode);

                using MemoryStream stream = new(fileBytes);

                ReportWriter writer = new()
                {
                    ReportProcessingMode = ProcessingMode.Remote
                };
                List<BoldReports.Web.ReportParameter> userParameters = new()
                    {
                        new BoldReports.Web.ReportParameter()
                        {
                            Name = "EventId",
                            Values = new List<string>() { dto.Id.ToString() }
                        }
                    };

                WriterFormat format = WriterFormat.PDF;

                writer.LoadReport(stream);
                writer.SetParameters(userParameters);
                MemoryStream memoryStream = new();
                writer.Save(memoryStream, format);

                string fileName = Path.Combine(Path.GetTempPath(), $@"certificate{dto.Id}.pdf");
                using (FileStream fileStream = new(fileName, FileMode.Create))
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
