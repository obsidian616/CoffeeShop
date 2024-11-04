using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoffeeShop.Models.ViewModels
{
    public class ForgotPasswordVm
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(256)]
        public string Email { get; set; }
    }
}