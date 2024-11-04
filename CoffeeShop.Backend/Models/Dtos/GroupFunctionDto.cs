﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.Dtos
{
    public class GroupFunctionDto
    {
        public int Id { get; set; }

        public int GroupId { get; set; }

        public int FunctionId { get; set; }

        public int ModifyUser { get; set; }

        public DateTime ModifyTime { get; set; }

        public bool Enabled { get; set; }

        public string GroupName { get; set; }
        public string FunctionName { get; set; }
    }
}