using HyperTaskCore.Services;
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
    [ServiceFilter(typeof(AuthorizeJwt))]
    public class UserController : ControllerBase
    {
        private IUserService UserService { get; set; }

        public UserController(FirebaseConnector connector,
                              CalendarTaskService calendarTaskService)
        {
            UserService = new UserService(connector, calendarTaskService);
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Get(string userId)
        {
            var user = await UserService.GetUserAsync(userId);

            return Ok(new DTOUser(user));
        }

        // PUT
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]DTOUser user)
        {
            var result = await UserService.InsertUpdateUserAsync(user);
            return Ok(result);
        }
    }
}