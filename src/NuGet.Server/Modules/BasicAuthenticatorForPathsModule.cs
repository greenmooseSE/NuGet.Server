using System;
using System.Web;
using SimpleBasicAuthentication;

namespace NuGet.Server.Modules
{
    ///<summary>Based on https://www.nuget.org/packages/SimpleBasicAuthenticationModule/ </summary>
    public class BasicAuthenticatorForPathsModule : IHttpModule
    {
        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public virtual void Init(HttpApplication context)
        {
            context.AuthenticateRequest += new EventHandler(this.context_AuthenticateRequest);
        }
        /// <summary>
        /// Handles the AuthenticateRequest event of the context control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
        protected virtual void context_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication httpApplication = (HttpApplication)sender;
            
            if (!IgnorePathsProvider.ShouldAuthenticate(httpApplication.Context))
                return;
            if (BasicAuthenticationProvider.Authenticate(httpApplication.Context, false))
                return;
            httpApplication.Context.Response.Status = "401 Unauthorized";
            httpApplication.Context.Response.StatusCode = 401;
            httpApplication.Context.Response.AddHeader("WWW-Authenticate", "Basic");
            httpApplication.CompleteRequest();
        }
        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.
        /// Leave empty!
        /// </summary>
        public void Dispose()
        {
        }
    }
}