using HyperTaskServices.Services;
using HyperTaskTools;
using HyperTaskWebApi.ActionFilterAttributes;
using HyperTaskWebApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
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
        private ReportService ReportService { get; set; }
        private MongoUserService UserService { get; set; }

        public ReportController(ReportService reportService,
                                MongoUserService userService)
        {
            ReportService = reportService;
            UserService = userService;
        }

        // Download all tasks in a csv
        [HttpGet]
        [RequestLimit("GetReport", NoOfRequest = 5, Seconds = 3600)]
        public async Task<IActionResult> Get(string userId)
        {
            await ValidateUserId(userId);

            var csv = await this.ReportService.GetTasksCsv(userId);

            byte[] fileBytes = Encoding.ASCII.GetBytes(csv);

            Stream stream = new MemoryStream(fileBytes);
            if (stream == null)
                return NotFound();

            return File(stream, "application/octet-stream", "hypertaskdata.csv");
        }

        private async Task ValidateUserId(string userId)
        {
            if (!await this.UserService.ValidateUserId(userId, this.Request.GetJwt()))
                throw new UnauthorizedAccessException("userId does not correspond to authenticated user");
        }
    }
}
