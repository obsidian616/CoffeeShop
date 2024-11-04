using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.ViewModels
{
    public class ForgotPasswordVm
    {
        [Display(Name = "電子信箱:")]
        [Required(ErrorMessage = "{0} 必填")]
        [StringLength(100)]
        public string Account { get; set; }
    }
}