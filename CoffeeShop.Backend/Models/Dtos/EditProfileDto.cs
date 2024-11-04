using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.Dtos
{
    public class EditProfileDto
    {
        public int Id { get; set; }

        public string Account { get; set; }
        public string Name { get; set; }


        public string Phone { get; set; }


        public bool IsStatus { get; set; }

      
        public IEnumerable<int> SelectedGroupIds { get; set; }
    }
}