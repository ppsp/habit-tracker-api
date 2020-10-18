using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperTaskWebApi.Helpers
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns the Authorization header's string, returns null if the request is null (for tests)
        /// </summary>
        public static string GetJwt(this HttpRequest request)
        {
            if (request == null)
                return null;

            return request.Headers["Authorization"].ToString();
        }
    }
}
