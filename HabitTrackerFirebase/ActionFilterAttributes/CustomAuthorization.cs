using HabitTrackerTools;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace HabitTrackerWebApi.ActionFilterAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizeJwt : Attribute, IAsyncAuthorizationFilter
    {
        public AuthorizeJwt()
        {

        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                var token = context.HttpContext.Request.Headers["Authorization"].ToString();

                var result = await HabitTrackerTools.FirebaseAdmin.Instance.ValidateJwt(token);

                if (!result)
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                else
                    return;
            }
            catch (Exception ex)
            {
                Logger.Debug("Error in OnAuthorizationAsync", ex);
            }
        }
    }
}
