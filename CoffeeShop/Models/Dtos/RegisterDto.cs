using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Models.Dtos
{
    public class RegisterDto
    {
        public int Id { get; set; }


        public string Name { get; set; }


        public bool Gender { get; set; }

        public string Phone { get; set; }


        public string Email { get; set; }


        public string Password { get; set; }



        //public string ConfirmPassword { get; set; }


        public DateTime Birthday { get; set; }

        public string ConfirmCode { get; set; }

        public string EncryptedPassword { get; set; }

        public bool? isConfirmed { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? ModifyTime { get; set; } // 可選屬性

        public bool EmailConfirmed { get; set; }    
    }
}