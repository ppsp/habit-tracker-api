using HyperTaskServices.Services;
using HyperTaskTools;
using HyperTaskWebApi.ActionFilterAttributes;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HyperTaskWebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [ServiceFilter(typeof(AuthorizeJwt))]
    public class ReportController : ControllerBase
    {
        private ReportService _ReportService { get; set; }

        public ReportController(ReportService reportService)
        {
            _ReportService = reportService;
        }

        // Download all tasks in a csv
        // TODO : Check that this can't be accessed for an other user
        [HttpGet]
        // [Route("api/Report/GetAllTasks")]
        public async Task<IActionResult> Get(string userId)
        {
            var csv = await this._ReportService.GetTasksCsv(userId);

            byte[] fileBytes = Encoding.ASCII.GetBytes(csv);

            // return File(fileBytes, "text/csv", "hypertask_data.csv");
            // return Ok();

            Stream stream = new MemoryStream(fileBytes);
            if (stream == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.

            return File(stream, "application/octet-stream", "hypertaskdata.csv");
        }
    }
}
