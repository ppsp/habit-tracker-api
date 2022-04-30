using HyperTaskCore.Services;
using HyperTaskServices.Models.DTO;
using HyperTaskServices.Services;
using HyperTaskTools;
using HyperTaskWebApi.ActionFilterAttributes;
using HyperTaskWebApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HyperTaskWebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [LogRequest]
    [ExceptionHandling]
    [ServiceFilter(typeof(AuthorizeJwt))]
    public class UserController : ControllerBase
    {
        private IUserService UserService { get; set; }

        public UserController(FirebaseConnector fireConnector,
                              MongoConnector mongoConnector,
                              MongoCalendarTaskService calendarTaskService,
                              MongoTaskGroupService taskGroupService)
        {
            UserService = new MongoUserService(mongoConnector, calendarTaskService, taskGroupService, fireConnector);
        }

        // GET
        [HttpGet]
        [RequestLimit("GetUser", NoOfRequest = 10000, Seconds = 3600)]
        public async Task<IActionResult> Get(string userId)
        {
            await ValidateUserId(userId);

            var user = await UserService.GetUserAsync(userId);

            return Ok(new DTOUser(user));
        }

        // PUT
        [HttpPut]
        [RequestLimit("PutUser", NoOfRequest = 10000, Seconds = 3600)]
        public async Task<IActionResult> Put([FromBody]DTOUser user)
        {
            await ValidateUserId(user.UserId);

            var result = await UserService.InsertUpdateUserAsync(user);
            return Ok(result);
        }

        // Delete
        [HttpDelete]
        [RequestLimit("DeleteUser", NoOfRequest = 1000, Seconds = 3600)]
        public async Task<IActionResult> Delete(string userId)
        {
            await ValidateUserId(userId);

            var user = await UserService.GetUserAsync(userId);
            var result = await UserService.PermaDeleteUser(user);
            return Ok(result);
        }

        private async Task ValidateUserId(string userId)
        {
            if (!await this.UserService.ValidateUserId(userId, this.Request.GetJwt()))
            {
                Logger.Error($"UserController UserId is invalid userId={userId} jwt={this.Request.GetJwt()}");
                throw new UnauthorizedAccessException("userId does not correspond to authenticated user");
            }
        }
    }
}