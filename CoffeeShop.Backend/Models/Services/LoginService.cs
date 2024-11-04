using CoffeeShop.Backend.Models.Components;
using CoffeeShop.Backend.Models.Dtos;
using CoffeeShop.Backend.Models.Interfaces;
using CoffeeShop.Backend.Models.Repositories;
using CoffeeShop.Backend.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.Services
{
    public class LoginService
    {
        private readonly IUserRepository _userRepository;

        public LoginService()
        {
            _userRepository = new UserRepository();


        }
        public LoginService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        }



        internal void ActiveRegister(int memberId, string confirmCode)
        {

            UserDto dto = _userRepository.Get(memberId);
            if (dto == null) throw new Exception("會員不存在");
            if (dto.ConfirmCode != confirmCode) throw new Exception("驗證碼錯誤");
            if (dto.IsConfirmed.HasValue && dto.IsConfirmed.Value) throw new Exception("會員已啟用");

            //啟用會員
            _userRepository.Active(memberId);
        }

        public List<int> GetGroupFuncids(string account)
        {
            return _userRepository.GetFuncIdsByUserId(account);

        }
        public UserDto GetUser(string account)
        {
            return _userRepository.Get(account);
        }
 
        internal Result ValidateLogin(LoginVm vm)
        {
            var user = _userRepository.Get(vm.Account);
            if (user == null) return Result.Fail("帳號或密碼錯誤");


            if (!user.IsConfirmed.HasValue || !user.IsConfirmed.Value)
            {
                return Result.Fail("帳號尚未啟用");
            }


            string hashedPassword = HashUtility.ToSHA256(vm.Password);
            if (hashedPassword != user.Password)
            {
                return Result.Fail("帳號或密碼錯誤");
            }

            return Result.Success();
        }
    }
}