using CoffeeShop.Backend.Models.Components;
using CoffeeShop.Backend.Models.Dtos;
using CoffeeShop.Backend.Models.Repositories;
using CoffeeShop.Backend.Models.Services;
using CoffeeShop.Backend.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var funcIds = service.GetGroupFuncids(account);


            var roles = string.Join(",", funcIds); 

            var user = service.GetUser(account);

            string userData = JsonConvert.SerializeObject(funcIds);

            FormsAuthenticationTicket ticket =
            new FormsAuthenticationTicket(
                1,         
                account,
                DateTime.Now, 
                DateTime.Now.AddHours(8),
                false,   
                roles,      
                "/"           
            );

            var value = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, value);


  
            var identity = new GenericIdentity(user.Name);
            var customPrincipal = new CustomPrincipal(identity, user.Id, user.Name, roles.Split(','));

            this.HttpContext.User = customPrincipal;

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
        [MyAuthorize]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [MyAuthorize]
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
        [MyAuthorize]
        public ActionResult ChangePasswordConfirm()
        {
            return View();
        }
        #endregion
    }
}
