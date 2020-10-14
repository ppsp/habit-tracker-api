using HyperTaskServices.Models.DTO;
using HyperTaskServices.Services;
using HyperTaskTools;
using HyperTaskWebApi.ActionFilterAttributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HyperTaskWebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [ServiceFilter(typeof(AuthorizeJwt))]
    public class TaskGroupController : ControllerBase
    {
        private TaskGroupService _TaskGroupService { get; set; }
        private UserService UserService { get; set; }

        public TaskGroupController(FirebaseConnector connector,
                                   CalendarTaskService calendarTaskService)
        {
            _TaskGroupService = new TaskGroupService(connector);
            UserService = new UserService(connector, calendarTaskService, _TaskGroupService);
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
            await UserService.UpdateLastActivityDate(group.UserId, group.UpdateDate ?? DateTime.Now);

            var result = await _TaskGroupService.InsertGroupAsync(group.ToTaskGroup());
            return Ok(result);
        }

        // PUT
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] DTOTaskGroup group)
        {
            await UserService.UpdateLastActivityDate(group.UserId, group.UpdateDate ?? DateTime.Now);

            var result = await _TaskGroupService.UpdateGroupAsync(group.ToTaskGroup());
            return Ok(result);
        }
    }
}