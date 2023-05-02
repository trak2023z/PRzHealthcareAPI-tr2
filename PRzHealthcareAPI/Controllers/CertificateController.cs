using BoldReports.Web.ReportViewer;
using BoldReports.Writer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRzHealthcareAPI.Models.DTO;
using PRzHealthcareAPI.Services;
using System.Data;

namespace PRzHealthcareAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [Microsoft.AspNetCore.Cors.EnableCors("FrontEndClient")]
    public class ReportViewerController : ControllerBase, IReportController
    {
        // Report viewer requires a memory cache to store the information of consecutive client request and
        // have the rendered Report Viewer information in server.
        private Microsoft.Extensions.Caching.Memory.IMemoryCache _cache;

        // IHostingEnvironment used with sample to get the application data from wwwroot.
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        // Post action to process the report from server based json parameters and send the result back to the client.
        public ReportViewerController(Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _cache = memoryCache;
            _hostingEnvironment = hostingEnvironment;
        }

        Dictionary<string, object> jsonArray = null;

        // Post action to process the report from server based json parameters and send the result back to the client.
        [HttpPost]
        public object PostReportAction([FromBody] Dictionary<string, object> jsonResult)
        {
            jsonArray = jsonResult;
            if (jsonResult.ContainsKey("parameters"))
            {
                //string sc = jsonResult["parameters"].ToString();
                //Params = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.PdfParams>(sc.Substring(1, sc.Length - 2));
            }

            return ReportHelper.ProcessReport(jsonResult, this, this._cache);
        }


        // Method will be called to initialize the report information to load the report with ReportHelper for processing.
        public void OnInitReportOptions(ReportViewerOptions reportOption)
        {
            var reportParameters = ReportHelper.GetParameters(jsonArray, this, _cache);

            string basePath = _hostingEnvironment.WebRootPath;
            reportOption.ReportModel.EmbedImageData = true;
            string reportName = "";

            if (reportOption.SubReportModel != null)
            {
                FileStream inputSubStream = new FileStream(basePath + @".\Resources\" + reportOption.SubReportModel.ReportPath + ".rdl", FileMode.Open, FileAccess.Read);
                MemoryStream SubStream = new MemoryStream();
                inputSubStream.CopyTo(SubStream);
                SubStream.Position = 0;
                inputSubStream.Close();
                reportOption.SubReportModel.Stream = SubStream;
                reportName = reportOption.SubReportModel.ReportPath;
            }
            else
            {
                FileStream inputStream = new FileStream(basePath + @".\Resources\" + reportOption.ReportModel.ReportPath + ".rdl", FileMode.Open, FileAccess.Read);
                MemoryStream reportStream = new MemoryStream();
                inputStream.CopyTo(reportStream);
                reportStream.Position = 0;
                inputStream.Close();
                reportOption.ReportModel.Stream = reportStream;
                reportName = reportOption.ReportModel.ReportPath;
            }
        }

        // Method will be called when reported is loaded with internally to start to layout process with ReportHelper.
        public void OnReportLoaded(ReportViewerOptions reportOption)
        {
            var reportParameters = ReportHelper.GetParameters(jsonArray, this, _cache);
            List<BoldReports.Web.ReportParameter> modifiedParameters = new List<BoldReports.Web.ReportParameter>();
            string reportName = "";
            if (reportOption.SubReportModel != null)
            {
                reportName = reportOption.SubReportModel.ReportPath;
            }
            else
            {
                reportName = reportOption.ReportModel.ReportPath;
            }

            if (reportParameters != null)
            {
                modifiedParameters.Add(new BoldReports.Web.ReportParameter()
                {
                    Name = "EventId",
                    Values = new List<string>() { "7195" }
                });
                reportOption.ReportModel.Parameters = modifiedParameters;

            }
        }


        //Get action for getting resources from the report
        [ActionName("GetResource")]
        [AcceptVerbs("GET")]
        // Method will be called from Report Viewer client to get the image src for Image report item.
        public object GetResource(ReportResource resource)
        {
            return ReportHelper.GetResource(resource, this, _cache);
        }

        [HttpPost]
        public object PostFormReportAction()
        {
            return ReportHelper.ProcessReport(null, this, _cache);
        }
    }
}