using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.ViewModels
{
    public class EditProfileVm
    {
        public int Id { get; set; }

        [Display(Name = "姓名:")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(50, ErrorMessage = "{0}長度不可大於{1}")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [Display(Name = "聯絡電話:")]
        [StringLength(10, ErrorMessage = "{0}長度不可大於{1}")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "在職狀態")]
        public bool IsStatus { get; set; }
        public List<int> SelectedGroupIds { get; set; } = new List<int>();
    }
}