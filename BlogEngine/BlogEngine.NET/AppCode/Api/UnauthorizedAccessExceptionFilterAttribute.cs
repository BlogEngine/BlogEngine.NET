using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace BlogEngine.NET.AppCode.Api
{
    public class UnauthorizedAccessExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is UnauthorizedAccessException)
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
        }
    }
}