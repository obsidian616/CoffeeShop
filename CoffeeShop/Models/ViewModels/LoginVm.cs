using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoffeeShop.Models.ViewModels
{
    public class LoginVm
    {
        //用信箱當帳號
        [Display(Name = "信箱")]
        [Required]
        public string Email { get; set; }

        [Display(Name = "密碼")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}