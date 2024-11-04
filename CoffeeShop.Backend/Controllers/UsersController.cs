using CoffeeShop.Backend.Models.Components;
using CoffeeShop.Backend.Models.Repositories;
using CoffeeShop.Backend.Models.Services;
using CoffeeShop.Backend.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoffeeShop.Backend.Controllers
{
    [MyAuthorize(Roles = "5")]
    public class UsersController : Controller
    {
        private readonly UserService _userService;
        private readonly UserGroupServices _userGroupServices;
 

        public UsersController()
        {

            _userService = new UserService(new UserRepository(), new UserGroupRepository("AppDbContext"));
            _userGroupServices = new UserGroupServices(new GroupRepository(), new UserGroupRepository("AppDbContext"));

        }

        public UsersController(UserService userService, UserGroupServices userGroupServices)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userGroupServices = userGroupServices ?? throw new ArgumentNullException(nameof(userGroupServices));
        }

        /// <summary>
        /// 使用者列表
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult GetAllUsers(string searchString)
        {
            var userDtos = _userService.SearchUsersByName(searchString);

            var userVms = userDtos.Select(dto => new RegisterVm
            {
                Id = dto.Id,
                Account = dto.Account,
                Name = dto.Name,
                Phone = dto.Phone,
                IsStatus = dto.IsStatus
            }).ToList();

            return Json(userVms, JsonRequestBehavior.AllowGet); 
        }

        // GET: Users/Register註冊
        public ActionResult Register()
        {
            var groups = _userGroupServices.GetAllGroups();
            ViewBag.Groups = groups;
            return View();
        }

        /// <summary>
        /// POST: Users/Register註冊
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="selectedGroups"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVm vm, int[] selectedGroups)
        {

            Result result = HandleRegister(vm, selectedGroups);


            if (result.IsSuccess)
            {
                TempData["ConfirmationUrl"] = result.Data; 
                return Json(new { success = true, redirectUrl = Url.Action("RegisterConfirm") }); 
            }


            ModelState.AddModelError(string.Empty, result.ErrorMessage);

   
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();

            return Json(new { success = false, errors = errors });
        }
        private Result HandleRegister(RegisterVm vm, int[] selectedGroups)
        {

            if (!ValidateRegister(vm, selectedGroups))
            {
                return Result.Fail("驗證失敗，請檢查輸入資料。");
            }

            try
            {

                var customPrincipal = (CustomPrincipal)User;
                int modifyId = customPrincipal.Id;
       
                bool isAccountExist = _userService.IsAccountExist(vm.Account);
                if (isAccountExist)
                {
                    return Result.Fail("帳號已存在");
                }

                int userId = _userService.Register(vm, selectedGroups, modifyId);


                var user = _userService.GetUserById(userId);
                if (user == null)
                {
                    return Result.Fail("使用者註冊失敗，無法找到對應的使用者。");
                }

                if (userId <= 0 || string.IsNullOrEmpty(user.ConfirmCode))
                {
                    return Result.Fail("無效的使用者 ID 或確認代碼");
                }

     
                string confirmationUrl = Url.Action("ActiveRegister", "Users", new { userId, confirmCode = user.ConfirmCode }, protocol: Request.Url.Scheme);

                return Result.Success(confirmationUrl);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
    
        [HttpGet]
        public JsonResult IsAccountExist(string account)
        {
            bool exists = _userService.IsAccountExist(account);
            return Json(new { exists }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 驗證註冊
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="selectedGroups"></param>
        /// <returns></returns>
        private bool ValidateRegister(RegisterVm vm, int[] selectedGroups)
        {
            if (!ModelState.IsValid)
            {
                return false; 
            }

            return true;
        }
        /// <summary>
        /// 處理啟用註冊
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confirmCode"></param>
        /// <returns></returns>
        private Result HandlerActiveRegister(int userId, string confirmCode)
        {
            try
            {
                _userService.ActiveRegister(userId, confirmCode);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult EditProfile(int id)
        {
            var userDto = _userService.GetUserById(id); 
            if (userDto == null) return HttpNotFound();

            var userGroups = _userGroupServices.GetUserGroups(id);
            if (userGroups == null) userGroups = new List<int>(); 

            var groups = _userGroupServices.GetAllGroups(); 
            ViewBag.Groups = new MultiSelectList(groups, "Id", "GroupName", userGroups);

         
            var vm = new EditProfileVm
            {
                Id = userDto.Id,
                Name = userDto.Name,
                Phone = userDto.Phone,
                IsStatus = userDto.IsStatus,
                SelectedGroupIds = userGroups
            };
            return View(vm);
        }

        //POST: Users/EditProfile
        [HttpPost]  
        public ActionResult EditProfile(EditProfileVm vm, int[] selectedGroupIds)
        {
      
            var result = HandleUpdateProfile(vm, selectedGroupIds);

            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "更新成功" });
            }

            return Json(new { success = false, message = result.ErrorMessage });
        }



        private Result HandleUpdateProfile(EditProfileVm vm, int[] selectedGroupIds)
        {
   
            if (!ModelState.IsValid)
            {
                return Result.Fail("資料不完整或無效。");
            }

            if (selectedGroupIds == null || selectedGroupIds.Length == 0)
            {
                return Result.Fail("未選擇任何群組");
            }

            try
            {
                var customPrincipal = (CustomPrincipal)User;
                int modifyId = customPrincipal.Id;

                // 調用服務進行個人資料更新
                var result = _userService.UpdateProfile(vm, selectedGroupIds, modifyId);

                if (result.IsSuccess)
                {
                    return Result.Success();
                }

                return Result.Fail(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public ActionResult RegisterConfirm()
        {
    
            if (TempData["ConfirmationUrl"] != null)
            {
                ViewBag.ConfirmationUrl = TempData["ConfirmationUrl"] as string;
            }
            else
            {
                ViewBag.ConfirmationUrl = "無法生成確認連結";
            }
            return View();
        }


        public ActionResult ActiveRegister(int userId, string confirmCode)
        {
            try
            {
                _userService.ActiveRegister(userId, confirmCode);
                ViewBag.Message = "帳號已成功啟用";
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View();
        }


    }
}