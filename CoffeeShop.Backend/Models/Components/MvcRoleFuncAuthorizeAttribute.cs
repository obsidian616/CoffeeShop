using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace CoffeeShop.Backend.Models.Components
{
    //[MvcRoleFuncAuthorize(FunctionId = "")]
    public class MvcRoleFuncAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public string FunctionId { get; set; }
        public string Roles { get; set; } 

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // 修改密碼頁面，允許所有已登錄用戶訪問
            if (httpContext.Request.Url.AbsolutePath.Contains("/Login/ChangePassword"))
            {
                return httpContext.User.Identity.IsAuthenticated;
            }
            if (HasFunctionRight(httpContext))
            {
                return true;
            }
            else if (!string.IsNullOrEmpty(FunctionId))
            {
                return false;
            }

        
            if (HasRoleAccess(httpContext))
            {
                return true;
            }

            return base.AuthorizeCore(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { Controller = "Login", Action = "NoPermission" }
                    )
                );
            }
            else
            {
                filterContext.Result = new RedirectResult("/Login/Login");
            }
        }

        private bool HasFunctionRight(HttpContextBase httpContext)
        {
            if (string.IsNullOrWhiteSpace(FunctionId)) return false;

            var claims = ((ClaimsPrincipal)httpContext.User).Claims
                          .Where(c => c.Type == "functionId").ToList();
            return claims.Any(c => c.Value == FunctionId);
        }

        private bool HasRoleAccess(HttpContextBase httpContext)
        {
            if (string.IsNullOrWhiteSpace(Roles)) return false;

            // 切割 Roles 字串並比對
            string[] roles = Roles.Split(',')
                                   .Select(r => r.Trim().ToLower())
                                   .Where(r => !string.IsNullOrEmpty(r))
                                   .ToArray();

            var claims = ((ClaimsPrincipal)httpContext.User).Claims
                          .Where(c => c.Type == ClaimTypes.Role).ToList();

            return claims.Any(c => roles.Contains(c.Value.ToLower()));
        }
    }
    public class ApiRoleFuncAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        public string FunctionId { get; set; }
        public string Roles { get; set; } 

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            //var requestUrl = actionContext.Request.RequestUri.AbsolutePath.ToLower();

            //// 如果是 ChangePassword 頁面，則忽略角色和功能檢查
            //if (requestUrl.Contains("/login/changepassword"))
            //{
            //    return true;
            //}
            if (HasFunctionRight(actionContext))
            {
                return true;
            }
            else if (!string.IsNullOrEmpty(FunctionId))
            {
                return false;
            }

            // 檢查 Roles
            if (HasRoleAccess(actionContext))
            {
                return true;
            }

            return base.IsAuthorized(actionContext);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            
            if (actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                // 已認證但無權限，回應重定向至 NoPermission
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
                actionContext.Response.Headers.Location = new Uri("/Login/NoPermission", UriKind.Relative);
            }
            else
            {
                // 未認證的情況下回應未授權狀態碼
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                actionContext.Response.Headers.Location = new Uri("/Login/Login", UriKind.Relative);
            }
        }

        private bool HasFunctionRight(HttpActionContext context)
        {
            if (string.IsNullOrWhiteSpace(FunctionId)) return false;

            var claims = ((ClaimsPrincipal)context.RequestContext.Principal).Claims
                          .Where(c => c.Type == "functionId").ToList();
            return claims.Any(c => c.Value == FunctionId);
        }

        private bool HasRoleAccess(HttpActionContext context)
        {
            if (string.IsNullOrWhiteSpace(Roles)) return false;

            // 切割 Roles 字串並比對
            string[] roles = Roles.Split(',')
                                   .Select(r => r.Trim().ToLower())
                                   .Where(r => !string.IsNullOrEmpty(r))
                                   .ToArray();

            var claims = ((ClaimsPrincipal)context.RequestContext.Principal).Claims
                          .Where(c => c.Type == ClaimTypes.Role).ToList();

            return claims.Any(c => roles.Contains(c.Value.ToLower()));
        }
    }
}