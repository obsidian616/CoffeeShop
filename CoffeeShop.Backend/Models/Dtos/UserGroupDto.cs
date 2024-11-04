using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.Dtos
{
    public class UserGroupDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int GroupId { get; set; }

        public int ModifyUser { get; set; }

        public DateTime ModifyTime { get; set; }
    }
}