using CoffeeShop.Backend.Models.Components;
using CoffeeShop.Backend.Models.Dtos;
using CoffeeShop.Backend.Models.Interfaces;
using CoffeeShop.Backend.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;


        public UserService(IUserRepository userRepository, IUserGroupRepository userGroupRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userGroupRepository = userGroupRepository ?? throw new ArgumentNullException(nameof(userGroupRepository));
        }

      
        public int Register(RegisterVm vm, int[] selectedGroupIds, int modifyId)
        {

            string salt = HashUtility.GetSalt();
            string hashedPassword = HashUtility.ToSHA256(vm.Password, salt);

            string confirmCode = Guid.NewGuid().ToString("N");


            var dto = new RegisterDto
            {
                Account = vm.Account,
                Password = hashedPassword,
                Name = vm.Name,
                Phone = vm.Phone,
                IsStatus = vm.IsStatus,
                ConfirmCode = confirmCode,
                CreatedTime = DateTime.Now,
                CreatedBy = modifyId,
                IsConfirmed = false,
                ModifyTime = DateTime.Now,
                ModifyUser = modifyId
            };


        
            int userId = _userRepository.Create(dto);


            if (selectedGroupIds != null && selectedGroupIds.Length > 0)
            {
                _userGroupRepository.AddUserGroups(userId, selectedGroupIds, dto.CreatedBy);
            }

            return userId; 
        }
        public bool IsAccountExist(string account)
        {
            return _userRepository.IsAccountExist(account);
        }

       
        internal void ActiveRegister(int userId, string confirmCode)
        {
            var user = _userRepository.Get(userId);
            if (user == null) throw new Exception("使用者不存在");
            if (user.ConfirmCode != confirmCode) throw new Exception("驗證碼錯誤");
            if (user.IsConfirmed.HasValue && user.IsConfirmed.Value) throw new Exception("帳號已啟用");


            _userRepository.Active(userId);
        }

      
        internal void ProcessChangePassword(string account, ChangePasswordDto dto)
        {
    
            var user = _userRepository.Get(account);

            string hashedOriginalPassword = HashUtility.ToSHA256(dto.OriginalPassword);
            if (hashedOriginalPassword != user.Password)
            {
                throw new Exception("原始密碼不正確");
            }

            hashedOriginalPassword = HashUtility.ToSHA256(dto.Password);
            user.Password = hashedOriginalPassword;



            _userRepository.UpdatePasswor(user);
        }

        public Result ProcessForgetPassword(string account, string urlTemplate, out string resetPasswordUrl)
        {

   
            var user = _userRepository.Get(account);
            if (user == null || !string.Equals(account, user.Account, StringComparison.CurrentCultureIgnoreCase))
            {
                resetPasswordUrl = null; 
                return Result.Fail("帳號或Email錯誤");
            }


            if (user.IsConfirmed == false)
            {
                resetPasswordUrl = null;
                return Result.Fail("您還沒有啟用本帳號,請先完成才能重設密碼");
            }

 
            var confirmCode = Guid.NewGuid().ToString("N");
            user.ConfirmCode = confirmCode;
            _userRepository.UpdateUser(user);


            resetPasswordUrl = string.Format(urlTemplate, user.Id, confirmCode);       
       
            return Result.Success();
        }
      
        internal void ProcessResetPassword(string account, ResetPasswordVm dto)
        {
            // 獲取使用者資料
            var user = _userRepository.Get(account);

            string hashedOriginalPassword = "";

            hashedOriginalPassword = HashUtility.ToSHA256(dto.Password);

            user.Password = hashedOriginalPassword;



            _userRepository.UpdatePasswor(user);
        }
        public UserDto GetUserByIdConfirmCode(int userId, string confirmCode)
        {
            return _userRepository.GetByIdAndConfirmCode(userId, confirmCode);
        }

       
        internal Result UpdateProfile(EditProfileVm vm, int[] selectedGroupIds, int modifyId)
        {
            var user = _userRepository.Get(vm.Id);
            if (user == null) return Result.Fail("使用者不存在");
            if (vm.Id == 1 && vm.IsStatus == false)
            {
                return Result.Fail("ID 為 1 的使用者無法被停用");
            }

            var dto = new UserDto
            {
                Id = vm.Id,
                Name = vm.Name,
                Phone = vm.Phone,
                IsStatus = vm.IsStatus,
                ModifyUser = modifyId,
                ModifyTime = DateTime.Now
            };
           
            if (vm.Id == 1)
            {
                selectedGroupIds = new[] { 1 }; 
            }
    
            if (selectedGroupIds != null && selectedGroupIds.Length > 0)
            {
                _userGroupRepository.UpdateUserGroups(vm.Id, selectedGroupIds, modifyId);
            }
            if (selectedGroupIds == null || selectedGroupIds.Length == 0)
            {
                return Result.Fail("未選擇任何群組");
            }

       
            foreach (var groupId in selectedGroupIds)
            {
                Console.WriteLine("Selected Group ID: " + groupId);
            }

  
            _userRepository.Update(dto);

            return Result.Success();
        }



        public UserDto GetUserById(int userId)
        {
            return _userRepository.Get(userId);
        }

        public IEnumerable<UserDto> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public IEnumerable<UserDto> SearchUsersByName(string searchString)
        {
            var users = _userRepository.GetAllUsers();

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.Name.Contains(searchString)).ToList();
            }

            return users;
        }

        public IEnumerable<UserDto> GetPagedUsers(int pageNumber, int pageSize, string searchString)
        {
            return _userRepository.GetPagedUsers(pageNumber, pageSize, searchString);
        }

        public int GetTotalUsersCount(string searchString)
        {
            return _userRepository.GetTotalUsersCount(searchString);
        }
    }
}