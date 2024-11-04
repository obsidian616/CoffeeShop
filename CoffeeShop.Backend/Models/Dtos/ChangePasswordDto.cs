using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.Dtos
{
    public class ChangePasswordDto
    {
        public string Password { get; set; }


        public string OriginalPassword { get; set; }
    }
}