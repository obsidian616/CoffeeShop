using CoffeeShop.Models.Dtos;
using CoffeeShop.Models.Infra;
using CoffeeShop.Models.Interfaces;
using CoffeeShop.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Models.Services
{
    public class MemberService
    {
        private IMemberRepository _repo;
        public MemberService()
        {
            _repo = new MemberRepository();
        }

        public MemberService(IMemberRepository repo)
        {
            _repo = repo;
        }





        public void Register(RegisterDto dto)
        {
            bool isEmailExist = _repo.IsEmailExist(dto.Email);
            if (isEmailExist) throw new Exception("帳號已存在");

            // 檢查是否是當月壽星
            bool isBirthdayMonth = dto.Birthday.Month == DateTime.Now.Month;

            string salt = HashUtility.GetSalt();
            string hashPassword = HashUtility.ToSHA256(dto.Password, salt);

            string confirmCode = Guid.NewGuid().ToString("N");

            dto.ConfirmCode = confirmCode;
            dto.EncryptedPassword = hashPassword;
            dto.isConfirmed = false;
            dto.CreateTime = DateTime.UtcNow.ToLocalTime(); // 設置創建時間
                                                           
            dto.EmailConfirmed = isBirthdayMonth; // 如果是當月壽星，設置 EmailConfirmed 為 true
            _repo.Create(dto);
        }

        internal void ActiveRegister(int memberId, string confirmCode)
        {
            MemberDto dto = _repo.Get(memberId);
            if (dto == null) throw new Exception("會員不存在");
            if (dto.ConfirmCode != confirmCode) throw new Exception("驗證碼錯誤");
            if (dto.IsConfirmed.HasValue &&
                dto.IsConfirmed.Value)
            {
                throw new Exception("會員已啟用");

            }

            //啟用會員
            _repo.Active(memberId);
        }

        internal void UpdatePassword(string account, ChangePasswordDto dto)
        {
            var memberInDb = _repo.Get(account);

            //驗證原始密碼
            string hashPassword = HashUtility.ToSHA256(dto.OriginPassword);
            if (hashPassword.CompareTo(memberInDb.EncryptedPassword) != 0)
            {
                throw new Exception("原始密碼錯誤");
            }

            hashPassword = HashUtility.ToSHA256(dto.ChangePassword);

            memberInDb.EncryptedPassword = hashPassword;

            // 更新修改時間
            memberInDb.ModifyTime = DateTime.Now;


            _repo.Update(memberInDb);
        }

        internal void UpdateProfile(EditProfileDto dto)
        {
            ////update name,email,mobile欄位值
            ////直接叫用EF，或者仍然叫用 IMemberRepository?
            //MemberDto memberInDb = _repo.Get(dto.Id);
            ////memberInDb.Name = dto.Name;
            //memberInDb.Gender = dto.Gender;
            //memberInDb.Phone = dto.Phone;
            //memberInDb.ModifyTime = DateTime.UtcNow.ToLocalTime();


            //_repo.Update(memberInDb);

            // 獲取資料庫中的現有會員資料
            var memberInDb = _repo.Get(dto.Id);
            if (memberInDb == null) throw new Exception("會員不存在");

            // 更新需要修改的屬性
            memberInDb.Name = dto.Name;  // 確保這個欄位也更新
            memberInDb.Gender = dto.Gender;
            memberInDb.Phone = dto.Phone;
            memberInDb.ModifyTime = DateTime.UtcNow; // 設置為當前的 UTC 時間

            // 更新會員資料
            _repo.UpdateMemberData(memberInDb);
        }

        internal Result ValidateLogin(LoginDto dto)
        {
            // 找出 User
            MemberDto member = _repo.Get(dto.Email);
            if (member == null)
                return Result.Fail("帳號或密碼錯誤"); // 會員不存在

            // 是否已啟用
            if (!member.IsConfirmed.HasValue || member.IsConfirmed.Value == false)
            {
                return Result.Fail("帳號尚未啟用");
            }

            // 將密碼雜湊後比對
            //var salt = HashUtility.GetSalt(); //補看看
            string hashPassword = HashUtility.ToSHA256(dto.Password);
            bool isPasswordCorrect = (hashPassword.CompareTo(member.EncryptedPassword) == 0);

            // 回傳結果
            if (isPasswordCorrect)
            {
                // 回傳成功結果
                return Result.Success();
            }
            else
            {
                return Result.Fail("帳號或密碼錯誤"); // 密碼錯誤
            }
        }
    }
}