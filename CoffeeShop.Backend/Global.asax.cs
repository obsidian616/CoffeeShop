using CoffeeShop.Backend.Models.Components;
using CoffeeShop.Backend.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace CoffeeShop.Backend
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var service = new LoginService();

            if (!Request.IsAuthenticated) return;


            if (User.Identity is FormsIdentity identity)
            {
                var authCookie = Context.Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];
                if (authCookie == null) return;

                var authTicket = System.Web.Security.FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket == null) return;

                // 取得 FormsAuthenticationTicket.UserData
                var userData = authTicket.UserData;
                var uName = authTicket.Name;
                string[] roles = authTicket.UserData.Split(',');
                var user = service.GetUser(uName);
                var customPrincipal = new CustomPrincipal(identity, user.Id, user.Name, roles);

         
                HttpContext.Current.User = customPrincipal;
                Context.User = customPrincipal; 

            }
        }
    }
}
