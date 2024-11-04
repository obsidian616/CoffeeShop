using CoffeeShop.Backend.Models.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoffeeShop.Backend.Controllers
{
    public class SidebarController : Controller
    {
        // GET: Sidebar
        public ActionResult Sidebar()
        {
            var sidebarMenu = new SidebarMenu();
            return PartialView("Sidebar", sidebarMenu.NavItems);
        }
    }
}

      