using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.Dtos
{
    public class RegisterDto
    {
        public int Id { get; set; }

        public string Account { get; set; }


        public string Password { get; set; }


        public string Name { get; set; }

        public string Phone { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedTime { get; set; }

        public int ModifyUser { get; set; }

        public DateTime ModifyTime { get; set; }

        public bool IsStatus { get; set; }

        public bool? IsConfirmed { get; set; }

        public string ConfirmCode { get; set; }

        public IEnumerable<int> SelectedGroupIds { get; set; }
    }
}