using CoffeeShop.Backend.Models.Components;
using CoffeeShop.Backend.Models.Dtos;
using CoffeeShop.Backend.Models.Repositories;
using CoffeeShop.Backend.Models.Services;
using CoffeeShop.Backend.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CoffeeShop.Backend.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserService _userService;
        public LoginController()
        {
  
            _userService = new UserService(new UserRepository(), new UserGroupRepository("AppDbContext"));

        }

        public LoginController(UserService userService, UserGroupServices userGroupServices)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

        }

        public ActionResult Login()
        {
            return View();

        }
        [HttpPost]
        public ActionResult Login(LoginVm vm)
        {
            if (ModelState.IsValid)
            {
                Result result = HandleLogin(vm);
                if (result.IsSuccess)
                {
                    (string url, HttpCookie cookie) = ProcessLogin(vm.Account);
                    Response.Cookies.Add(cookie);

                    return Redirect(url);

                }
                ModelState.AddModelError(
                    string.Empty,
                    result.ErrorMessage);
            }
            return View(vm);
        }


        private (string url, HttpCookie cookie) ProcessLogin(string account)
        {
            var service = new LoginService();

            // 假設 service.GetGroupFuncids(account) 返回功能 ID 列表
            var funcIds = service.GetGroupFuncids(account);

            // 使用功能 ID 列表作為角色
            var roles = string.Join(",", funcIds);

            // 取得用戶資料
            var user = service.GetUser(account);

            // 將功能 ID 列表序列化為 JSON 字串
            string userData = JsonConvert.SerializeObject(funcIds);

            // 建立 FormsAuthenticationTicket
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1,          // 版本號
                account,    // 用戶名（通常是帳號）
                DateTime.Now, // 發行時間
                DateTime.Now.AddHours(8), // 失效時間
                false,      // 是否記住我
                roles,      // 用戶的角色（或功能ID）
                "/"         // Cookie 的路徑
            );

            // 加密票據並創建 cookie
            var value = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, value);

            // 創建 ClaimsIdentity 並添加所需的聲明
            var identity = new ClaimsIdentity("CustomAuthentication");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())); // 用戶 ID
            identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "someProvider")); // 身份提供者（可自訂）

            // 創建 CustomPrincipal 並設置到 HttpContext.User
            var customPrincipal = new CustomPrincipal(identity, user.Id, user.Name, roles.Split(','));
            this.HttpContext.User = customPrincipal;

            // 取得重定向 URL
            var url = FormsAuthentication.GetRedirectUrl(account, true);

            return (url, cookie);
        }

        private Result HandleLogin(LoginVm vm)
        {
            try
            {
                var service = new LoginService();
                Result validateResult = service.ValidateLogin(vm);
                return validateResult;
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }


        // 顯示「無權限」頁面
        public ActionResult NoPermission()
        {
            return View();
        }

        [HttpGet]
        [Route("Logout")]
        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();

            return RedirectToAction("Login");
        }


      
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPasswordVm vm)
        {


            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            string resetLink;
            var urlTemplate = $"{Request.Url.Scheme}://{Request.Url.Authority}/Login/ResetPassword?userId={{0}}&confirmCode={{1}}";

            var result = _userService.ProcessForgetPassword(vm.Account, urlTemplate, out resetLink);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(vm);
            }

            return RedirectToAction("ForgotPasswordConfirm", new { resetLink });
        }
        public ActionResult ForgotPasswordConfirm(string resetLink)
        {
            ViewBag.ResetLink = resetLink;
            return View();
        }

        #region 重置密碼

        public ActionResult ResetPassword()
        {
            int userId = Convert.ToInt32(Request.QueryString["userId"]);
            string confirmCode = Request.QueryString["confirmCode"];
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(int userId, string confirmCode, ResetPasswordVm vm)
        {

            var user = _userService.GetUserByIdConfirmCode(userId, confirmCode);

            Result result = HandleResetPassword(user.Account, vm);
            if (result.IsSuccess)
            {
                return RedirectToAction("Login");
            }
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View();
        }
        /// <summary>
        /// 重置密碼流程
        /// </summary>
        /// <param name="account"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        private Result HandleResetPassword(string account, ResetPasswordVm vm)
        {
            try
            {
  
                _userService.ProcessResetPassword(account, vm);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        #endregion

        #region 更改密碼


        [MvcRoleFuncAuthorize]
        public ActionResult ChangePassword()
        {
            return View();
        }


        [MvcRoleFuncAuthorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordVm vm)
        {
            string account = User.Identity.Name;
            Result result = HandleChangePassword(account, vm);
            if (result.IsSuccess)
            {
                return RedirectToAction("ChangePasswordConfirm");
            }
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View();
        }
        /// <summary>
        /// 更改密碼流程
        /// </summary>
        /// <param name="account"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        private Result HandleChangePassword(string account, ChangePasswordVm vm)
        {

            try
            {
              
                var dto = new ChangePasswordDto
                {

                    OriginalPassword = vm.OriginalPassword,
                    Password = vm.Password
                };

       
                _userService.ProcessChangePassword(account, dto);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
 
    
        public ActionResult ChangePasswordConfirm()
        {
            return View();
        }
        #endregion
    }
}
