using HyperTaskTools;
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
    /// <summary>    
    /// Action filter to restrict limit on no. of requests per IP address.    
    /// </summary>    
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute" />    
    [AttributeUsage(AttributeTargets.Method)]
    public class RequestLimitAttribute : ActionFilterAttribute
    {
        public RequestLimitAttribute(string name)
        {
            Name = name;
        }

        public string Name
        {
            get;
        }

        public int NoOfRequest
        {
            get;
            set;
        } = 1;

        public int Seconds
        {
            get;
            set;
        } = 1;

        private static MemoryCache Cache
        {
            get;
        } = new MemoryCache(new MemoryCacheOptions());

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var ipAddress = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
            var memoryCacheKey = $"{Name}-{ipAddress}";
            Cache.TryGetValue(memoryCacheKey, out int prevReqCount);
            if (prevReqCount >= NoOfRequest * 10) // *10 for safety
            {
                context.Result = new ContentResult
                {
                    Content = $"Request limit is exceeded. Try again in {Seconds} seconds.",
                };
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                Logger.Error($"Request limit is exceeded for {Name} ip = {ipAddress}");
            }
            else
            {
                if (prevReqCount == NoOfRequest * 5)
                {
                    Logger.Warn($"Request limit reached half for {Name} ip = {ipAddress}");
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(Seconds));
                Cache.Set(memoryCacheKey, (prevReqCount + 1), cacheEntryOptions);
            }
        }
    }
}
