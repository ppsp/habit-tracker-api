﻿using HyperTaskCore.Models;
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
    [ServiceFilter(typeof(AuthorizeJwt))]
    public class TaskHistoryController : ControllerBase
    {
        private TaskHistoryService TaskHistoryService { get; set; }
        private FireUserService UserService { get; set; }

        public TaskHistoryController(FirebaseConnector connector,
                                     FireCalendarTaskService calendarTaskService,
                                     FireTaskGroupService taskGroupService)
        {
            TaskHistoryService = new TaskHistoryService(calendarTaskService);
            UserService = new FireUserService(connector, calendarTaskService, taskGroupService);
        }

        // POST
        [HttpPost]
        [RequestLimit("PostHistory", NoOfRequest = 500, Seconds = 3600)]
        public async Task<IActionResult> Post([FromBody]TaskHistory task)
        {
            Logger.Debug("posting taskhistory id:" + task.CalendarTaskId);

            await ValidateUserId(task.UserId);

            await UserService.UpdateLastActivityDate(task.UserId, task.UpdateDate ?? DateTime.Now);

            var result = await TaskHistoryService.InsertHistoryAsync(task);
            return Ok(result);
        }

        // PUT
        [HttpPut]
        [RequestLimit("PutHistory", NoOfRequest = 200, Seconds = 3600)]
        public async Task<IActionResult> Put([FromBody]TaskHistory task)
        {
            Logger.Debug("putting taskhistory id:" + task.CalendarTaskId);

            await ValidateUserId(task.UserId);

            await UserService.UpdateLastActivityDate(task.UserId, task.UpdateDate ?? DateTime.Now);

            var result = await TaskHistoryService.UpdateHistoryAsync(task);
            return Ok(result);
        }

        private async Task ValidateUserId(string userId)
        {
            if (!await this.UserService.ValidateUserId(userId, this.Request.GetJwt()))
                throw new UnauthorizedAccessException("userId does not correspond to authenticated user");
        }
    }
}