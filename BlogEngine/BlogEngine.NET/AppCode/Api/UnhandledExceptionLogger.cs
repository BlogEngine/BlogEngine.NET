using System;
using System.Web.Http.ExceptionHandling;
using BlogEngine.Core;

namespace BlogEngine.NET.AppCode.Api
{
    public class UnhandledExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            if (context.Exception is UnauthorizedAccessException) { return; }

            Utils.Log("{0} {1}: {2}", context.Request.Method, context.Request.RequestUri, context.Exception.Message);
        }
    }
}