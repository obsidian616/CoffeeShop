using CoffeeShop.Backend.Models.EFModels;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using CoffeeShop.Backend.Models.ViewModels;
using CoffeeShop.Backend.Models.Components;

namespace CoffeeShop.Backend.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // 主頁
      
        public ActionResult Index()
        {
            return View();
        }

        [MyAuthorize(Roles = "8")]
        // 聯絡我們頁面，顯示 GuestBook 資料
        public ActionResult Contact(int? page)
        {
            var guestBooks = db.GuestBooks.ToList(); // 從資料庫中取得所有資料
            int pageSize = 10;
            int pageNumber = (page ?? 1); // 如果page為null，則頁碼設為1
            var pagedList = guestBooks.ToPagedList(pageNumber, pageSize);

            return View(pagedList); // 傳遞分頁後的資料到視圖
        }

        [MyAuthorize(Roles = "8")]
        // 刪除留言
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var guest = db.GuestBooks.Find(id);
            if (guest == null)
            {
                return HttpNotFound(); //status code 404
            }

            db.GuestBooks.Remove(guest);  // 刪除留言
            db.SaveChanges();  // 保存更改

            TempData["SuccessMessage"] = "留言已成功刪除";  // 可以添加一個成功提示

            return RedirectToAction("Contact");  // 重定向回留言板
        }

        [MyAuthorize(Roles = "7")]
        // 餐點和會員分析
        public ActionResult MenuAndMemberAnalysis(DateTime? startDate, DateTime? endDate)
        {
            DateTime filterStartDate = startDate ?? DateTime.Now.AddMonths(-1);  // 默認最近一個月
            DateTime filterEndDate = endDate ?? DateTime.Now;

            // 會員分析 - 新會員和回訪會員數量
            int newMemberCount = db.Members.Where(m => m.CreateTime >= filterStartDate && m.CreateTime <= filterEndDate).Count();

            int returningMemberCount = db.Orders
                .Where(o => o.OrderDate >= filterStartDate && o.OrderDate <= filterEndDate)
                .GroupBy(o => o.MemberId)
                .Where(g => g.Count() > 1)
                .Count();

            // 餐點銷售分析 - 按訂單分組，每個訂單的所有產品及其數量
            var orderDetails = db.OrderItems
                .Where(oi => oi.Order.OrderDate >= filterStartDate && oi.Order.OrderDate <= filterEndDate && oi.Order.Status == 1)
                .GroupBy(oi => oi.OrderId)
                .Select(g => new
                {
                    OrderId = g.Key,
                    Products = g.Select(oi => new
                    {
                        oi.ItemCName,
                        Quantity = oi.Quantity
                    }).ToList()
                }).ToList();

            // 計算每個產品的總銷售數量
            var productSales = orderDetails
                .SelectMany(o => o.Products)
                .GroupBy(p => p.ItemCName)
                .Select(g => new
                {
                    ItemCName = g.Key,
                    TotalQuantity = g.Sum(p => p.Quantity)
                })
                .ToDictionary(g => g.ItemCName, g => g.TotalQuantity);

            // 使用 ViewBag 將數據傳遞給視圖
            ViewBag.NewMemberCount = newMemberCount;
            ViewBag.ReturningMemberCount = returningMemberCount;
            ViewBag.ProductSales = Newtonsoft.Json.JsonConvert.SerializeObject(productSales);
            ViewBag.StartDate = filterStartDate;
            ViewBag.EndDate = filterEndDate;

            return View();
        }

    }
}

