using CoffeeShop.Backend.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public Order Order { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}