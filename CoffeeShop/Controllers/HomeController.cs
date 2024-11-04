using CoffeeShop.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoffeeShop.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact([Bind(Include ="Name,Email,Phone,Message")]GuestBook guestbook)
        {
            if (ModelState.IsValid)
            {
                using (var db = new AppDbContext())
                {
                    guestbook.Createdtime = DateTime.Now;
                    db.GuestBooks.Add(guestbook);
                    db.SaveChanges();
                }
                ViewBag.SuccessMessage = "您的訊息已成功儲存！";
                return View();
            }
            else
            {
                ViewBag.ErrorMessage = "請確認你的填入資料";
            }
            // 你可以在這裡返回原頁面或跳轉到其他頁面
            return View(guestbook);
        }

        public ActionResult Test()
        {
            return View();
        }
    }
}