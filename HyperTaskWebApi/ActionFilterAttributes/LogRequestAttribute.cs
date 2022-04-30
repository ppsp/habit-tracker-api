using HyperTaskTools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HyperTaskWebApi.ActionFilterAttributes
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method]
    public class LogRequestAttribute : ActionFilterAttribute
    {
        public LogRequestAttribute()
        {
           
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Logger.Debug($"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path} ");
        }
    }
}
