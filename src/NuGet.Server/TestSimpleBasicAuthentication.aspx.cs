using System;

namespace NuGet.Server.nuget
{
    public partial class TestSimpleBasicAuthentication : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Name: " + Context.User.Identity.Name;
            Title += "; Is authenticated: " + Context.User.Identity.IsAuthenticated.ToString();

            if (Context.User.Identity.IsAuthenticated)
            {
                Title += "; Has User role: " + Context.User.IsInRole("User");
                Title += "; Has Admin role: " + Context.User.IsInRole("Admin");
            }
        }
    }
}