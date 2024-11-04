using CoffeeShop.Backend.Models.Components;
using CoffeeShop.Backend.Models.EFModels;
using CoffeeShop.Backend.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoffeeShop.Backend.Controllers
{
    
    public class PurchaseController : Controller
    {
        private readonly AppDbContext _context;

        public PurchaseController()
        {
            _context = new AppDbContext();  // 手動初始化資料庫上下文
        }

        [MyAuthorize(Roles = "3")]
        public ActionResult Index(int page = 1, string searchMonth = null)
        {
            int pageSize = 10; // 每頁顯示的訂單數量
            var ordersQuery = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(searchMonth))
            {
                DateTime selectedMonth;
                if (DateTime.TryParseExact(searchMonth, "yyyy-MM", null, System.Globalization.DateTimeStyles.None, out selectedMonth))
                {
                    DateTime startDate = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);
                    DateTime endDate = startDate.AddMonths(1);

                    // 篩選訂單日期在指定月份內的訂單
                    ordersQuery = ordersQuery.Where(o => o.OrderDate >= startDate && o.OrderDate < endDate);
                }
            }

            // 分頁
            var orders = ordersQuery
                           .OrderBy(o => o.OrderDate)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

            // 計算總頁數
            int totalOrders = ordersQuery.Count();
            ViewBag.PageNumber = page;
            ViewBag.PageCount = (int)Math.Ceiling((double)totalOrders / pageSize);
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < ViewBag.PageCount;
            ViewBag.SearchMonth = searchMonth; // 用於保持當前搜索條件

            return View(orders);
        }

        [MyAuthorize(Roles = "3")]
        // 顯示訂單明細
        public ActionResult Details(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);
            var orderItems = _context.OrderItems.Where(oi => oi.OrderId == id).ToList();

            if (order == null)
            {
                return HttpNotFound();
            }

            var viewModel = new OrderDetailsViewModel
            {
                Order = order,
                OrderItems = orderItems
            };

            return View(viewModel);
        }

        [MyAuthorize(Roles = "1")]
        //進行中的訂單
        public ActionResult OrderProcessing()
        {
            // 抓取狀態為 1 的進行中訂單
            var activeOrders = _context.Orders
                                 .Include("OrderItems") // 包含關聯的訂單項目
                                 .Where(o => o.Status == 2)
                                 .ToList();

            return View(activeOrders);
        }

        [MyAuthorize(Roles = "3")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateStatus(int id, int status)
        {
            try
            {
                // 查詢對應的訂單
                var order = _context.Orders.FirstOrDefault(o => o.Id == id);
                if (order == null)
                {
                    return Json(new { success = false, message = "訂單未找到" });
                }

                // 更新訂單狀態
                order.Status = status;
                order.Modifytime = DateTime.Now; // 更新最後修改時間
                _context.SaveChanges(); // 保存修改

                return Json(new { success = true, message = "訂單狀態已更新" });
            }
            catch (Exception ex)
            {
                // 錯誤處理
                return Json(new { success = false, message = "更新失敗", error = ex.Message });
            }
        }

        [MyAuthorize(Roles = "3")]
        [HttpPost]
        public ActionResult Complete(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                order.Status = 1; // 假設 2 代表完成
                _context.SaveChanges();
            }
            return Json(new { success = true });
        }

        [MyAuthorize(Roles = "3")]
        [HttpPost]
        public ActionResult Cancel(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                order.Status = 3; // 假設 3 代表取消
                _context.SaveChanges();
            }
            return Json(new { success = true });
        }

    }

}
