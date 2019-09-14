// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;
using NLog;
using NLog.Config;
using NuGet.Server.DataServices;
using NuGet.Server.Infrastructure;
using NuGet.Server.V2;

// The consuming project executes this logic with its own copy of this class. This is done with a .pp file that is
// added and transformed upon package install.
#if DEBUG
[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NuGet.Server.App_Start.NuGetODataConfig), "Start")]
#endif

namespace NuGet.Server.App_Start
{
    public static class NuGetODataConfig
    {
        private static Logger _logger;

        public static void Start()
        {
            ServiceResolver.SetServiceResolver(new DefaultServiceResolver());

            Initialize(GlobalConfiguration.Configuration, "PackagesOData");
        }

        public static void Initialize(HttpConfiguration config, string controllerName, bool initLogging = true)
        {
            NuGetV2WebApiEnabler.UseNuGetV2WebApiFeed(
                config,
                "NuGetDefault",
                "nuget",
                controllerName,
                enableLegacyPushRoute: true);

            //config.Services.Replace(typeof(IExceptionLogger), new TraceExceptionLogger());
            config.Services.Replace(typeof(IExceptionLogger), new NLogExceptionLogger());
            
            // Trace.Listeners.Add(new TextWriterTraceListener(HostingEnvironment.MapPath("~/NuGet.Server.log")));
            // Trace.AutoFlush = true;
            if (initLogging)
                LogManager.LoadConfiguration(HostingEnvironment.MapPath("~/nlog.config"));
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _logger.Debug("Logging initialized");
            config.Routes.MapHttpRoute(
                name: "NuGetDefault_ClearCache",
                routeTemplate: "nuget/clear-cache",
                defaults: new { controller = controllerName, action = nameof(PackagesODataController.ClearCache) },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );
        }
    }
}
