using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.ExceptionHandling;
using NLog;

namespace NuGet.Server.Infrastructure
{
    public class NLogExceptionLogger : ExceptionLogger
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        public override void Log(ExceptionLoggerContext context)
        {
            _log.Error(context.Exception, "Error for request {Request}.", RequestToString(context.Request));
        }

        private static string RequestToString(HttpRequestMessage request)
        {
            var message = new StringBuilder();
            if (request.Method != null)
                message.Append(request.Method);

            if (request.RequestUri != null)
                message.Append(" ").Append(request.RequestUri);

            return message.ToString();
        }
    }
}