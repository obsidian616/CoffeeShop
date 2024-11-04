using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.ViewModels
{
    public class LoginVm
    {
        [Display(Name = "帳號:")]
        [Required(ErrorMessage = "{0}必填")]
        public string Account { get; set; }

        [Display(Name = "密碼:")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0}必填")]
        public string Password { get; set; }
    }
}