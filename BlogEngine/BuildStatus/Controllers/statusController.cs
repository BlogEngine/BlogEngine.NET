using BuildStatus.Poco;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BuildStatus.Controllers
{
    [Route("_apis/build/[controller]")]
    public class statusController : Controller
    {
        private object lockObject = new object();

        [HttpGet("{definitionId}")]
        public async Task<IActionResult> Get(string definitionId)
        {
            var args = new RequestEventArgs
            {
                Url = Program.Configuration["AppSettings:Url"],
                ProjectName = Program.Configuration["AppSettings:ProjectName"],
                PersonalAccessToken = "YOUR-PAT-GOES-HERE",
                DefinitionId = definitionId
            };

            var result = await GetBuildStatus(this, args);
            return File(result.ResponseBody, "image/svg+xml");
        }

        public async Task<ServiceResult> GetBuildStatus(object sender, EventArgs e)
        {
            var args = e as RequestEventArgs;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", "", args.PersonalAccessToken))));

                    using (HttpResponseMessage response = await client.GetAsync(
                                $"{args.Url}/{args.ProjectName}/_apis/build/status/{args.DefinitionId}"))
                    {
                        response.EnsureSuccessStatusCode();
                        var result = new ServiceResult
                        {
                            ResponseBody = await response.Content.ReadAsByteArrayAsync()
                        };
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    FriendlyMessage = ex.Message,
                    DeveloperMessage = ex.StackTrace,
                    IsError = true,
                    ResponseBody = System.IO.File.ReadAllBytes("Error.svg")
                };
            }
        }
    }
}
