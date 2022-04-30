using HyperTaskServices.Models.DTO;
using HyperTaskServices.Services;
using HyperTaskTools;
using HyperTaskWebApi.ActionFilterAttributes;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HyperTaskWebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [ExceptionHandling]
    [LogRequest]
    [ServiceFilter(typeof(AuthorizeJwt))]
    public class BugReportController : ControllerBase
    {
        private BugReportService bugReportService { get; set; }

        public BugReportController(AzureDevopsConnector connector)
        {
            this.bugReportService = new BugReportService(connector);
        }

        // POST
        [HttpPost]
        [RequestLimit("PostBugReport", NoOfRequest = 200, Seconds = 3600)]
        public async Task<IActionResult> Post([FromBody]DTOBugReport report)
        {
            var result = await this.bugReportService.CreateAzureDevopsWorkItemAsync(report);

            return Ok(result);
        }
    }
}