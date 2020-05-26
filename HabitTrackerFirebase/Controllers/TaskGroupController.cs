using HabitTrackerServices.Models.DTO;
using HabitTrackerServices.Services;
using HabitTrackerTools;
using HabitTrackerWebApi.ActionFilterAttributes;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace HabitTrackerWebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [ServiceFilter(typeof(AuthorizeJwt))]
    public class TaskGroupController : ControllerBase
    {
        private TaskGroupService _TaskGroupService { get; set; }

        public TaskGroupController(FirebaseConnector connector)
        {
            _TaskGroupService = new TaskGroupService(connector);
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Get(string userId)
        {
            var groups = await _TaskGroupService.GetGroupsAsync(userId);

            return Ok(groups.Select(p => DTOTaskGroup.FromTaskGroup(p)).ToList());
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]DTOTaskGroup group)
        {
            var result = await _TaskGroupService.InsertGroupAsync(group.ToTaskGroup());
            return Ok(result);
        }

        // PUT
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] DTOTaskGroup group)
        {
            var result = await _TaskGroupService.UpdateGroupAsync(group.ToTaskGroup());
            return Ok(result);
        }
    }
}