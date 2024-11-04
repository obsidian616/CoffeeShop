using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Models.Dtos
{
    public class EditProfileDto
    {
        public int Id { get; set; }
        public bool Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string Name { get; set; }
    }
}