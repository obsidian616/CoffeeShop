using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CoffeeShop.Backend.Models.Components
{
    public class MyAuthorizeAttribute: AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {

           
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
               
                CustomPrincipal currentUser = filterContext.HttpContext.User as CustomPrincipal;
                if (currentUser == null) return;

               
                if (string.IsNullOrEmpty(Roles)) return;

                string[] allowFunctions = Roles.Split(',').Select(f => f.Trim()).ToArray();

              
                if (allowFunctions.Any(f => currentUser.IsInRole(f))) return;

              
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new { Controller = "Login", Action = "NoPermission" })
                );

                return;


            }
           
            filterContext.Result = new RedirectResult("/Login/Login");

           
        }
    }
}