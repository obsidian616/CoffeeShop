using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Models.Dtos
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public bool EmailConfirmed { get; set; }
    }
}