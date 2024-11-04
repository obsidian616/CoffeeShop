using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShop.Backend.Models.Components
{
    public class SidebarMenu
    {
        public class NavItem
        {
            public string Name { get; set; }
            public string Icon { get; set; }
            public string Link { get; set; }
            public List<string> Roles { get; set; } 
            public List<NavItem> SubItems { get; set; } 

            public NavItem(string name, string icon, string link, List<string> roles, List<NavItem> subItems = null)
            {
                Name = name;
                Icon = icon;
                Link = link;
                Roles = roles;
                SubItems = subItems;
            }
        }

        public List<NavItem> NavItems { get; set; }

        public SidebarMenu()
        {
            NavItems = new List<NavItem>
            {
                new NavItem("通知", "fa-regular fa-bell", "/Purchase/OrderProcessing", new List<string> { "1" }),
                new NavItem("訂單", "fa-solid fa-shopping-cart", "#orders", new List<string> { "3" }, new List<NavItem>
                {
                    new NavItem("歷史紀錄", "fa-solid fa-list", "/Purchase/Index", new List<string> { "3" }),

                }),
                new NavItem("菜單", "fa-solid fa-utensils", "#menu", new List<string> { "2" }, new List<NavItem>
                {
                    new NavItem("維護", "fa-solid fa-clipboard-list",  "/Orders/Index", new List<string> { "2" }),
                    new NavItem("菜單分類", "fa-solid fa-clipboard-list",  "/Orders/CreateCategory", new List<string> { "2" }),


                }),
                new NavItem("會員", "fa-solid fa-users", "/Members", new List<string> { "4" }),
                new NavItem("管理", "fa-solid fa-cogs", "#", new List<string> { "5", "6" }, new List<NavItem>
                {
                    new NavItem("員工清單", "fa-solid fa-user-tie", "/Users/Index", new List<string> { "5" }),
                    new NavItem("新增員工", "fa-solid fa-user-plus", "/Users/Register", new List<string> { "5" }),
                    new NavItem("功能權限", "fa-solid fa-user-lock", "/GroupFunctions/Index", new List<string> { "6" }),
                    new NavItem("群組管理", "fa-solid fa-users-cog", "/Groups/Index", new List<string> { "6" })
                }),

                new NavItem("分析", "fa-solid fa-chart-line", "/Home/MenuAndMemberAnalysis", new List<string> { "7" }),
                new NavItem("留言", "fa-solid fa-comments","/Home/Contact", new List<string> { "8" })
            };
        }
    }
}
