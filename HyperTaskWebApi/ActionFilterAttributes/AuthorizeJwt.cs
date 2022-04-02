using HyperTaskCore.Exceptions;
using HyperTaskTools;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace HyperTaskWebApi.ActionFilterAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizeJwt : Attribute, IAsyncAuthorizationFilter
    {
        private FirebaseConnector Connector { get; set; }

        public AuthorizeJwt(FirebaseConnector connector)
        {
            Connector = connector;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                var token = context.HttpContext.Request.Headers["Authorization"].ToString();

                var result = await Connector.ValidateJwt(token);

                if (!result)
                    throw new UnauthorizedAccessException("Unable to validate Json Web Token");
                else
                    return;
            }
            catch (InvalidJwtTokenException x)
            {
                throw new UnauthorizedAccessException();
            }
            catch (Exception ex)
            {
                Logger.Error("Unknown error while validating Json Web Token", ex);
                throw new UnauthorizedAccessException("Unknown error while validating Json Web Token");
                throw;
            }
        }
    }
}
