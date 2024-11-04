using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.Dtos
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public int CreatedBy { get; set; }
        public int ModifyUser { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifyTime { get; set; }
        public bool Enabled { get; set; }
    }
}