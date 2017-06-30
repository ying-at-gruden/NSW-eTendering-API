using System.Web.Mvc;
using System.Threading.Tasks;
using Api.Web.Models;
using Api.Web.Utilities;
using TikaOnDotNet.TextExtraction;
using System.Configuration;
using Api.Utilities.FileStorage;
using Api.Utilities.ViewHelpers;
using Api.Web.Models.S3;
using System.Collections.Generic;

namespace Api.Web.Controllers
{
    public class HomeController : Controller
    {
        public AwsStorageClient AwsClient { get; set; }

        public FormatHelper FormatHelper { get; set; }

        //private EtrDbContext db = new EtrDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExtractFile()
        {
            var textExtractor = new TextExtractor();
            //var result = textExtractor.Extract(@"d:\Downloads\AusTender User Interface Guide V1.13.pdf");

            AwsClient.BucketName = ConfigurationManager.AppSettings["aws:s3:documents_bucket"];
            var fileList = new FileList() { Files = new List<S3File>() };

            var bucketFileList = AwsClient.List("*.*"); // List all files in the bucket
            foreach(var filename in bucketFileList)
            {
                var fileStream = AwsClient.ReadFile(filename); // Read file stream from S3 file
                var fileBytes = FormatHelper.convertStreamToByteArray(fileStream);
                var result = textExtractor.Extract(fileBytes); // Extract file content
                fileList.Files.Add(new S3File()
                {
                    Filename = filename,
                    ContentType = result.ContentType,
                    MetaData = (Dictionary<string,string>)result.Metadata,
                    FileContent = result.Text
                });
            }

            return View(fileList);
        }

        public async Task<ActionResult> ParsePlannedProcurements()
        {
            //var cn = new cn() { CNUUID = "F7EABE30-D193-79E8-4EA6EC13105DE46C" };
            //db.SaveChanges();
            var requestHelper = new RequestHelper();
            var baseUrl = "http://ec2-54-252-210-13.ap-southeast-2.compute.amazonaws.com/";
            //var baseUrl = "http://dev.etr.gruden.com/";
            var result = "<table><tr><th>Planned Procurement</th><th>Tenders</th><th>Contracts</th></tr>";

            // valid search result              ?event=public.api.planning.search&ResultsPerPage=99
            // fail with "errors"               ?event=public.api.tender.view&RFTUUID=DCBEFDDE-CFF9-430D-4DFAE339EA77430
            eTenderAPIResponse jsonResponse = await requestHelper.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.planning.search&ResultsPerPage=99");

            foreach (Release release in jsonResponse.releases)
            {
                var tender = release.tender;
                string ppUuid = tender.PlannedProcurementUUID;
                eTenderAPIResponse plannedProcurement = await requestHelper.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.planning.view&plannedProcurementUUID=" + ppUuid);
                await Task.Delay(3000);

                var rftArray = tender.relatedRFT;
                var rftCount = rftArray?.Count ?? 0;
                var ppTender = plannedProcurement.releases[0].tender;
                result += "<tr><td rowspan='" + rftCount + "'><a href=\"" + baseUrl + "?event=public.api.planning.view&plannedProcurementUUID=" + ppUuid + "\">";
                result += ppTender.id + "</a><br/>Estimated Date of Approach to Market: " + ppTender.estimatedDateToMarket + "</td>";

                if (rftCount > 0)
                {
                    var rftIndex = 0;
                    foreach (var rftUuid in rftArray)
                    {
                        result += "<td>";
                        eTenderAPIResponse rft = await requestHelper.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.tender.view&RFTUUID=" + rftUuid);
                        var rftTender = rft.releases[0].tender;
                        result += "<a href=\"" + baseUrl + "?event=public.rft.show&RFTUUID=" + rftUuid + "\">";
                        result += rftTender.title + "</a><br/>Published: " + rftTender.tenderPeriod.startDate + "</td>";
                        result += "<td>";
                        if (rftTender.relatedCN != null)
                        {

                            foreach (var cnUuid in rftTender.relatedCN)
                            {
                                eTenderAPIResponse contract = await requestHelper.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.contract.view&CNUUID=" + cnUuid);
                                result += "<a href='" + baseUrl + "?event=public.cn.view&CNUUID=" + cnUuid + "'>";
                                result += contract.releases[0].awards[0].title + "</a><br/>";
                                // the server will block your IP address if you make too many responses in a short time
                                await Task.Delay(3000);
                            }
                        }
                        if (rftTender.relatedSON != null)
                        {
                            foreach (var sonUuid in rftTender.relatedSON)
                            {
                                eTenderAPIResponse contract = await requestHelper.GetAndDecode<eTenderAPIResponse>(baseUrl + "?event=public.api.contract.view&SONUUID=" + sonUuid);
                                result += "<a href='" + baseUrl + "?event=public.SON.view&SONUUID=" + sonUuid + "'>";
                                result += contract.releases[0].awards[0].title + "</a><br/>";
                                await Task.Delay(3000);
                            }

                        }
                        result += "</td>";

                        if (rftIndex != rftCount - 1)
                        {
                            result += "</tr><tr>";
                        }
                        rftIndex++;

                    }
                }
                else
                {
                    result += "<td>No Tenders</td><td></td>";
                }
                result += "</tr>";
            }

            result += "</table>";

            ViewBag.Result = result;
            return View();
        }
    }
}