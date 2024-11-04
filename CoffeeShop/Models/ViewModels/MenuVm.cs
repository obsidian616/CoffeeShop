using CoffeeShop.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Models.ViewModels
{
    public class MenuVm
    {
        public int CategoryId { get; set; }
        public string CategoryName{ get; set; }
        public List<Menu> Menus { get; set; }
    }
}