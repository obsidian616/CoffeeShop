using CoffeeShop.Models.Dtos;
using CoffeeShop.Models.EFModels;
using CoffeeShop.Models.Infra;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoffeeShop.Models.ViewModels
{
    public class RegisterVm
    {
        public int Id { get; set; }

        [Display(Name = "姓名")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Display(Name = "性別")]
        [Required]
        public bool Gender { get; set; }

        [Display(Name = "手機")]
        [Required]
        [StringLength(10)]
        public string Phone { get; set; }

        [Required]
        [StringLength(256)]
        [EmailAddress]
        public string Email { get; set; }


        [Display(Name = "密碼")]
        [Required]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "確認密碼")]
        [Required]
        [StringLength(50)]
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        //public DateTime CreateTime { get; set; }
    }


    public static class RegisterExt
    {
        public static Member ToMember(this RegisterVm vm)
        {
            var salt = HashUtility.GetSalt();
            var hashPassword = HashUtility.ToSHA256(vm.Password, salt);
            //var hashPassword = HashUtility.ToSHA256(vm.Password);
            var confirmCode = Guid.NewGuid().ToString("N");

            return new Member
            {
                Id = vm.Id,
                Name = vm.Name,
                Phone = vm.Phone,
                Email = vm.Email,
                Birthday = vm.Birthday,
                EncryptedPassword = hashPassword, //補上這段試試看
                IsConfirmed = false,
                Gender = vm.Gender,
                ConfirmCode = confirmCode,
                CreateTime = DateTime.UtcNow.ToLocalTime(),
            };
        }

        public static RegisterDto ToDto(this RegisterVm vm)
        {
            return new RegisterDto
            {
                Id = vm.Id,
                Name = vm.Name,
                Gender = vm.Gender,
                Phone = vm.Phone,
                Password = vm.Password,
                Email = vm.Email,
                Birthday = vm.Birthday,
                CreateTime = DateTime.UtcNow.ToLocalTime(), // 設置為當前時間
            };
        }
    }
}