using HabitTrackerCore.Services;
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
    public class UserController : ControllerBase
    {
        private IUserService UserService { get; set; }

        public UserController(FirebaseConnector connector)
        {
            UserService = new UserService(connector);
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