using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoffeeShop.Models.ViewModels
{
    public class EditProfileVm
    {
        public int Id { get; set; }

        //public string Email { get; set; }

        //[Display(Name = "姓名")]
        //[Required]
        //[StringLength(30)]
        //public string Name { get; set; }

        [Display(Name = "手機號碼")]
        [StringLength(10)]
        public string Phone { get; set; }

        public bool Gender { get; set; }
    }
}