using CoffeeShop.Models;
using CoffeeShop.Models.EFModels;
using CoffeeShop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CoffeeShop.Controllers
{

    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController()
        {
            _context = new AppDbContext();  // 手動初始化資料庫上下文
        }

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // 顯示菜單頁面
        public ActionResult Index()
        {
            var returnModel = new List<MenuVm>();
            // 查詢啟用的菜單分類
            var categories = _context.MenuCategories
                                     .Where(c => c.Enabled == true)
                                     .ToList();

            // 查詢啟用的菜單產品，並將其對應到相應的分類
            foreach (var category in categories)
            {

                returnModel.Add(new MenuVm()
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    Menus = _context.Menus
                                        .Where(m => m.Enabled == true && m.CategoryID == category.Id)
                    .ToList()
                });
            }      
            // 傳遞到視圖
            return View(returnModel);
        }

        public ActionResult Order(int? id)  // 使用 int? 允許 id 為 null
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);  // 如果 id 為 null，返回 400 錯誤
            }

            var menu = _context.Menus.FirstOrDefault(m => m.Id == id);
            if (menu == null)
            {
                return HttpNotFound();  // 如果找不到對應的商品，返回 404
            }

            return View(menu);  // 傳遞商品信息到視圖
        }

        public ActionResult Cart()
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            // 計算購物車中的總數量
            ViewBag.CartItemCount = cart.Sum(i => i.Quantity);

            return View(cart);
        }

        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            var menuItem = _context.Menus.FirstOrDefault(m => m.Id == id);
            if (menuItem == null)
            {
                return HttpNotFound();  // 如果找不到商品，返回 404
            }

            // 從 Session 中獲取購物車，如果購物車不存在則創建一個新的
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            // 檢查購物車中是否已經包含該商品
            var existingCartItem = cart.FirstOrDefault(c => c.ProductId == id);
            if (existingCartItem != null)
            {
                // 如果商品已經存在，則更新數量
                existingCartItem.Quantity += quantity;
            }
            else
            {
                // 如果商品不存在，則新增商品到購物車
                var cartItem = new CartItem
                {
                    ProductId = menuItem.Id,
                    ItemEName = menuItem.ItemEName,  // 新增此行來設置英文名稱
                    ProductName = menuItem.ItemCName,
                    Price = menuItem.Price,
                    Quantity = quantity,
                    ImageFileName = menuItem.FileName
                };

                cart.Add(cartItem);
            }

            // 更新 Session
            Session["Cart"] = cart;

            return RedirectToAction("Cart");  // 重定向到購物車頁面
        }

        public JsonResult GetCartItemCount()
        {
            // 從 Session 中讀取購物車資料
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            // 計算購物車中所有商品的總數量
            int totalQuantity = cart.Sum(item => item.Quantity);

            // 返回商品數量
            return Json(new { count = totalQuantity }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RemoveFromCart(int productId)
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            // 找到要移除的產品
            var itemToRemove = cart.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);  // 從購物車中移除該產品
            }

            // 更新 Session
            Session["Cart"] = cart;

            // 返回刪除成功的消息和更新後的購物車總數量
            var totalItemCount = cart.Sum(i => i.Quantity);

            return Json(new { success = true, totalItemCount = totalItemCount });
        }

        [HttpPost]
        public JsonResult UpdateCartQuantity(int productId, int quantity)
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity; // 更新數量
                item.TotalPrice = item.Price * item.Quantity; // 更新總價
            }

            // 計算購物車中的總數量
            var totalItemCount = cart.Sum(i => i.Quantity);

            // 更新 Session
            Session["Cart"] = cart;

            return Json(new { success = true, totalPrice = item.TotalPrice.ToString("C"), totalItemCount = totalItemCount });
        }

        [HttpPost]
        public ActionResult CreateOrder(string tableNumber, List<CartItem> items,string discountcode)
        {
            if (string.IsNullOrEmpty(tableNumber) || items == null || !items.Any() || items.Sum(x => x.Quantity) == 0)
            {
                return Json(new { success = false, message = "桌號或購物車為空。" });
            }
            //從cookie取得登入資訊
            var cookieValue = Request.Cookies[FormsAuthentication.FormsCookieName]?.Value as string;
            int? memberid = null;
            int? discountTotalPrice = null;
            if (cookieValue != null)
            {
                var Email = FormsAuthentication.Decrypt(cookieValue).Name;
                var member = _context.Members.FirstOrDefault(x => x.Email == Email);
                var canusecoupon = member.EmailConfirmed;
                memberid = member?.Id;

                // 檢查是否有輸入優惠碼
                if (!string.IsNullOrEmpty(discountcode))
                {
                    // 檢查是否為訪客
                    if (memberid == null)
                    {
                        // 設定錯誤提醒，訪客無法使用優惠碼
                        return Json(new { success = false, message = "訪客無法使用優惠碼，請登入會員。" });
                    }

                    // 檢查是否允許使用優惠碼
                    if (canusecoupon == true)
                    {
                        if (discountcode == "SPECIAL10")
                        {
                            // 計算折扣總價
                            discountTotalPrice = (int)items.Sum(i => i.TotalPrice * 0.9m);
                            member.EmailConfirmed = false;

                            // 設定提醒消息
                            TempData["DiscountMessage"] = "恭喜! 優惠卷使用成功，此次訂單有打九折!";
                        }
                        else
                        {
                            // 設定失敗的提醒消息，並返回不創建訂單
                            return Json(new { success = false, message = "套用失敗，請檢查您的優惠碼!" });
                        }
                    }
                    else
                    {
                        // 設定失敗的提醒消息，並返回不創建訂單
                        return Json(new { success = false, message = "套用失敗，請檢查您的優惠碼!" });
                    }
                }
            }
            else
            {
                // 訪客情況，無法使用優惠碼
                if (!string.IsNullOrEmpty(discountcode))
                {
                    return Json(new { success = false, message = "訪客無法使用優惠碼，請登入會員。" });
                }
            }

            // 創建新的訂單
            var order = new Order
            {
                TableNumber = tableNumber,
                OrderDate = DateTime.Now,
                Status = 2,  // 設定訂單狀態
                TotalAmount = items.Sum(i => (int)i.TotalPrice),  // 使用 CartItem 的 TotalPrice 屬性計算總金額
                DiscountTotalAmount = discountTotalPrice,
                Createdtime = DateTime.Now,
                Modifytime = DateTime.Now,
                MemberId = memberid
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            // 創建並保存每個訂單項目
            foreach (var item in items.Where(x => x.Quantity > 0))
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    MenuId = item.ProductId,
                    Price = (int)item.Price,
                    ItemCName = item.ProductName,
                    ItemEName = item.ItemEName,
                    TotalAmount = (int)item.TotalPrice,
                    Quantity = item.Quantity,
                    Notes = item.Remark
                };

                _context.OrderItems.Add(orderItem);
            }

            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult ClearCart()
        {
            // 假設購物車的 SESSION 名稱為 "Cart"
            Session.Remove("Cart");

            // 其他想重置的 SESSION 可以在這裡加入
            //Session.Clear(); // 若需要清空所有 SESSION，可以使用這行

            return Json(new { success = true });
        }

    }

}
