using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.ViewModels
{
    public class GroupVm
    {
        public int Id { get; set; }

        [Display(Name = "群組名稱:")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(50)]
        public string GroupName { get; set; }

        [Required]

        public bool Enabled { get; set; }
    }
}