using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using CoffeeShop.Models;
using CoffeeShop.Models.Dtos;
using CoffeeShop.Models.EFModels;
using CoffeeShop.Models.Infra;
using CoffeeShop.Models.Repositories;
using CoffeeShop.Models.Services;
using CoffeeShop.Models.ViewModels;

namespace CoffeeShop.Controllers
{
    public class MembersController : Controller
    {
        public ActionResult Index()
        {
            return View();

        }
        // GET: Members
        public ActionResult Register()
        {
            var model = new RegisterVm(); // 確保初始化模型
            return View(model);
        }

        [HttpPost]
        public ActionResult Register(RegisterVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            Result result = HandleRegister(vm);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(vm);
            }

            return View("RegisterConfirm");
        }

        public ActionResult ActiveRegister(int memberId, string confirmCode)
        {
            Result result = HandleActiveRegister(memberId, confirmCode);
            return View();
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
                    (string url, HttpCookie cookie) = ProcessLogin(vm.Email); //建立方法

                    Response.Cookies.Add(cookie);
                    //return Redirect(url);
                    //return RedirectToAction("Index", "Members");
                    return Redirect("/Orders/Index"); // 固定跳轉到首頁
                }
                ModelState.AddModelError(
                    string.Empty,
                    result.ErrorMessage);
            }
            return View(vm);
        }

        public ActionResult EditProfile()
        {
            var account = User.Identity.Name;
            MemberDto dto = new MemberRepository().Get(account);

            EditProfileVm vm = MvcApplication._mapper.Map<EditProfileVm>(dto);
            return View(vm);
            //return View();

        }

        [Authorize]
        [HttpPost]
        public ActionResult EditProfile(EditProfileVm vm)
        {
            string account = User.Identity.Name;
            Result result = HandleUpdateProfile(account, vm);

            if (result.IsSuccess)
            {
                return RedirectToAction("MemberProfile"); // 更新成功，回會員中心頁
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(vm);
        }


        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(ChangePasswordVm vm)
        {
            string account = User.Identity.Name;
            Result result = HandleChangePassword(account, vm);

            if (result.IsSuccess)
            {
                return RedirectToAction("MemberProfile"); // 更新成功，回會員編輯頁
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(vm);
        }

        private Result HandleLogin(LoginVm vm)
        {
            try
            {
                var service = new MemberService();

                LoginDto dto = MvcApplication._mapper.Map<LoginDto>(vm);

                Result validateResult = service.ValidateLogin(dto);

                return validateResult;
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Members");
        }

        private Result HandleActiveRegister(int memberId, string confirmCode)
        {
            try
            {
                var service = new MemberService();
                service.ActiveRegister(memberId, confirmCode);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        private Result HandleRegister(RegisterVm vm)
        {
            MemberService service = new MemberService();

            try
            {
                RegisterDto dto = vm.ToDto();
                service.Register(dto);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        private (string url, HttpCookie cookie) ProcessLogin(string account)
        {
            var roles = string.Empty; // 角色欄位，沒有用到角色權限，所以存入空白

            // 建立一張認證憑證
            var ticket = new FormsAuthenticationTicket(
                1,                              // 版本別，沒有特別用途
                account,                        // 使用者帳號
                DateTime.Now,                   // 發行日
                DateTime.Now.AddDays(2),        // 到期日
                false,                          // 是否記住
                roles,                          // 使用者資料
                "/"                             // cookie 位置
            );

            // 將它加密
            var value = FormsAuthentication.Encrypt(ticket);

            // 存入 cookie
            var cookies = new HttpCookie(FormsAuthentication.FormsCookieName, value);

            // 取得 return url
            var url = FormsAuthentication.GetRedirectUrl(account, true); // 第二個引數沒有用途

            return (url, cookies);

        }







        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordVm vm)
        {
            if (ModelState.IsValid == false) return View(vm);

            var urlTemplate = Request.Url.Scheme + "://" + //生成http:.//或https://
                            Request.Url.Authority + "/" +//生成網域名稱或ip
                            "Members/ResetPassword?memberid={0}&confirmCode={1}"; //生成網頁url

            var result = ProcessResetPassword(vm.Email, urlTemplate);

            if (result.IsSuccess == false)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(vm);
            }
            return View("ForgotPasswordConfirm");
        }

        private Result ProcessResetPassword(string email, string urlTemplate)
        {
            var db = new AppDbContext();
            // 檢查 account, email 正確性
            var memberInDb = db.Members.FirstOrDefault(m => m.Email == email);

            if (memberInDb == null) return Result.Fail("帳號或 Email 錯誤"); // 故意不告知錯誤錯誤原因
            if (string.Compare(email, memberInDb.Email, StringComparison.CurrentCultureIgnoreCase) != 0) return Result.Fail("帳號或 Email 錯誤");

            // 檢查 IsConfirmed 必須為 true，因為只有已啟用帳號才能重設密碼
            if (memberInDb.IsConfirmed == false) return Result.Fail("你還沒有啟用本帳號,請先完成才能重設密碼");

            // 更新紀錄，導入 confirmCode
            var confirmCode = Guid.NewGuid().ToString("N");
            memberInDb.ConfirmCode = confirmCode;
            db.SaveChanges();

            // 發 email
            var url = string.Format(urlTemplate, memberInDb.Id, confirmCode);
            new EmailHelper().SendForgotPasswordEmail(url, memberInDb.Name, email);

            return Result.Success();
        }

        private Result HandleUpdateProfile(string Email, EditProfileVm vm)
        {
            var service = new MemberService();
            try
            {
                EditProfileDto dto = MvcApplication._mapper.Map<EditProfileDto>(vm);
                service.UpdateProfile(dto);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }

        }

        public ActionResult ResetPassword(int memberId, string confirmCode)
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordVm vm, int memberId, string confirmCode)
        {
            if (ModelState.IsValid == false) return View(vm);

            Result result = ProcessChangePassword(memberId, confirmCode, vm.Password);

            if (result.IsSuccess == false)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(vm);
            }
            return View("ResetPasswordConfirm");
        }

        private Result ProcessChangePassword(int memberId, string confirmCode, string password)
        {
            var db = new AppDbContext();

            // 驗證 memberId 和 confirmCode 是否正確
            var memberInDb = db.Members.FirstOrDefault(m => m.Id == memberId && m.ConfirmCode == confirmCode);
            if (memberInDb == null) return Result.Fail("找不到對應的會員紀錄");

            // 更新密碼，並將 confirmCode 清空
            var salt = HashUtility.GetSalt();
            var encryptedPassword = HashUtility.ToSHA256(password, salt);

            memberInDb.EncryptedPassword = encryptedPassword;
            memberInDb.ConfirmCode = null;
            // 更新修改時間
            memberInDb.ModifyTime = DateTime.Now;

            db.SaveChanges();

            return Result.Success();
        }


        private Result HandleChangePassword(string account, ChangePasswordVm vm)
        {
            var service = new MemberService();
            try
            {
                ChangePasswordDto dto = MvcApplication._mapper.Map<ChangePasswordDto>(vm);
                service.UpdatePassword(account, dto);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public ActionResult MemberProfile()
        {
            var email = User.Identity.Name;
            var memberDto = new MemberRepository().Get(email); // 确保 Get 方法返回一个 MemberDto

            if (memberDto == null)
            {
                return HttpNotFound(); // 如果找不到用户资料
            }

            return View(memberDto); // 将用户资料传递给视图
        }

        public ActionResult Birthdaycoupon()
        {
            var email = User.Identity.Name;
            var memberDto = new MemberRepository().Get(email); // 獲取用戶資料

            if (memberDto == null)
            {
                return HttpNotFound(); // 如果沒有這個會員
            }

            // 獲取當前生日月份
            var currentMonth = DateTime.Now.Month;

            // 檢查會員的生日月份
            var isBirthdayMonth = memberDto.Birthday.Month == currentMonth;

            // 判斷是否可以使用優惠券
            if (memberDto.EmailConfirmed == false)
            {
                ViewBag.CanUseCoupon = false; 
            }
            else
            {
                ViewBag.CanUseCoupon = CanUseCoupon(memberDto, isBirthdayMonth);
            }

            // 將生日結果傳到View
            ViewBag.IsBirthdayMonth = isBirthdayMonth;

            return View(memberDto);
        }

        public ActionResult UseCoupon()
        {
            var email = User.Identity.Name;
            var memberDto = new MemberRepository().Get(email);

            if (memberDto == null)
            {
                return HttpNotFound(); // 如果沒有這個會員
            }

            // 初始化 ViewBag.CanUseCoupon
            ViewBag.CanUseCoupon = memberDto.EmailConfirmed; // 如果 EmailConfirmed 為 true，表示可以使用優惠券

            // 檢查 TempData 中是否有消息
            ViewBag.Message = TempData["Message"] as string;

            // 將會員資料傳遞到視圖
            return View(memberDto);
        }

        [HttpPost]
        public ActionResult UseCoupon(string discountCode)
        {
            var email = User.Identity.Name;
            var memberDto = new MemberRepository().Get(email);

            if (memberDto == null)
            {
                return HttpNotFound(); // 如果沒有這個會員
            }

            // 檢查會員是否已經使用過優惠券
            if (!memberDto.EmailConfirmed) // 當 EmailConfirmed 為 false 時表示已經使用過
            {
                TempData["Message"] = "您已經使用過此優惠券！";
                return RedirectToAction("UseCoupon"); // 返回使用優惠券頁面
            }

            // 在這裡可以添加對折扣碼的檢查
            if (discountCode == "SPECIAL10") // 假設的折扣碼檢查
            {
                // 更新會員資料（假設使用優惠券後標記為使用過）
                memberDto.EmailConfirmed = false; // 標記為已使用
                new MemberRepository().Update(memberDto); // 更新資料庫
                ViewBag.CanUseCoupon = false;
                return Json(new { success = true, message = "優惠券已成功使用！" });
            }
            else
            {
                return Json(new { success = false, message = "無效的優惠券。" });
            }
        }


        // 新增的公用方法，用於檢查會員是否可以使用優惠券
        private bool CanUseCoupon(MemberDto memberDto, bool isBirthdayMonth = false)
        {
            return isBirthdayMonth && memberDto.EmailConfirmed == true;
        }



    }
}
