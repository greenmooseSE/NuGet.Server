using System;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using NLog;

namespace NuGet.Server.Infrastructure
{
    public class LogActionAttribute : ActionFilterAttribute
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var controller = actionContext.RequestContext.RouteData.Values["Controller"];
            var action = actionContext.RequestContext.RouteData.Values["Action"];
            var fullActionName = $"{controller}.{action}";
            MappedDiagnosticsContext.Set("ActionName", fullActionName);
            actionContext.Request.Properties.Add("ActionStart", DateTime.Now.Ticks);
            actionContext.Request.Properties.Add("ActionName", fullActionName);
            _log.Trace("Action {Action} started.", fullActionName);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionContext)
        {
            if (actionContext.Request.Properties.TryGetValue("ActionStart", out var ticks))
                if (actionContext.Request.Properties.TryGetValue("ActionName", out var actionName))
                {
                    var duration = TimeSpan.FromTicks(DateTime.Now.Ticks - (long) ticks);
                    _log.Trace("Action {Action} finished after {ActionDuration}.", actionName, duration);
                }

            base.OnActionExecuted(actionContext);
        }
    }
}