using HyperTaskCore.Utils;
using HyperTaskTools;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;

namespace HyperTaskWebApi.ActionFilterAttributes
{
    public class ExceptionHandling : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            // SETUP CHAIN OF RESPONSIBILITY
            UnauthorizedHandler unauthorizedHandler = new UnauthorizedHandler();
            BusinessExceptionHandler businessExceptionHandler = new BusinessExceptionHandler();
            CriticalExceptionHandler criticalExceptionHandler = new CriticalExceptionHandler();

            unauthorizedHandler.SetSuccessor(businessExceptionHandler);
            businessExceptionHandler.SetSuccessor(criticalExceptionHandler);

            // PROCESS REQUEST
            unauthorizedHandler.ProcessRequest(context);
        }

        public abstract class APIExceptionHandler
        {
            protected APIExceptionHandler Successor;

            public void SetSuccessor(APIExceptionHandler _successor)
            {
                Successor = _successor;
            }

            public abstract void ProcessRequest(HttpActionExecutedContext context);
        }

        public class UnauthorizedHandler : APIExceptionHandler
        {
            public override void ProcessRequest(HttpActionExecutedContext context)
            {
                if (context.Exception is HttpResponseException && ((HttpResponseException)context.Exception).Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent(context.Exception.Message),
                        ReasonPhrase = context.Exception.Message,
                    });
                }
                else
                {
                    Successor.ProcessRequest(context);
                }
            }
        }

        public class BusinessExceptionHandler : APIExceptionHandler
        {
            public override void ProcessRequest(HttpActionExecutedContext context)
            {
                if (context.Exception.GetType().In(GetAllBusinessExceptionTypes()))
                {
                    Logger.Info(String.Format("Business exception in method : {0}", context.Request.GetActionDescriptor().ActionName), context.Exception);

                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(context.Exception.Message),
                        ReasonPhrase = context.Exception.Message,
                    });
                }
                else
                {
                    Successor.ProcessRequest(context);
                }
            }
        }

        public class CriticalExceptionHandler : APIExceptionHandler
        {
            public override void ProcessRequest(HttpActionExecutedContext context)
            {
                if (context.Exception.Message.Contains("Reading from the stream has failed") || context.Exception is OperationCanceledException)
                {
                    Logger.Warn(String.Format("Unhandled exception in method : {0}", context.Request.GetActionDescriptor().ActionName), context.Exception);
                }
                else
                {
                    Logger.Error(String.Format("Unhandled exception in method : {0}", context.Request.GetActionDescriptor().ActionName), context.Exception);
                }

                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred, please try again or contact the administrator."),
                    ReasonPhrase = "Internal Server Exception"
                });
            }
        }

        public static List<Type> GetAllBusinessExceptionTypes()
        {
            List<Type> businessExceptions = new List<Type>();

            return businessExceptions;
        }
    }
}
