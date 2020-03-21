using HabitTrackerServices.Models.DTO;
using HabitTrackerServices.Services;
using HabitTrackerTools;
using HabitTrackerWebApi.ActionFilterAttributes;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HabitTrackerWebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
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
        public async Task<IActionResult> Post([FromBody]DTOBugReport report)
        {
            var result = await this.bugReportService.CreateAzureDevopsWorkItemAsync(report);

            return Ok(result);
        }
    }
}