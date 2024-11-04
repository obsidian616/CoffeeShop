using CoffeeShop.Backend.Models.Components;
using CoffeeShop.Backend.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CoffeeShop.Backend.Controllers
{
    [MyAuthorize(Roles = "2")]
    public class OrdersController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Index(int? categoryId, int page = 1)
        {
            int pageSize = 10;  // 每頁顯示 10 個項目
            var query = db.Menus.Include(m => m.MenuCategory).OrderBy(m => m.Id);  // 查詢菜單，並包括分類資料

            // 如果有傳入分類 ID，進行篩選
            if (categoryId.HasValue)
            {
                query = (IOrderedQueryable<Menu>)query.Where(m => m.CategoryID == categoryId.Value);
            }

            var totalItems = query.Count();  // 總項目數量
            var menus = query
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

            // 計算總頁數
            int pageCount = (int)Math.Ceiling(totalItems / (double)pageSize);

            // 傳遞分頁資訊到視圖
            ViewBag.PageNumber = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageCount = pageCount;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < pageCount;

            // 傳遞所有分類到視圖以生成篩選表單
            ViewBag.Categories = new SelectList(db.MenuCategories.ToList(), "Id", "Name");
            ViewBag.SelectedCategoryId = categoryId;

            return View(menus);  // 傳遞篩選後的菜單項目
        }

        // GET: Create
        public ActionResult Create()
        {

            // 傳遞分類列表到 ViewBag.Categories
            ViewBag.Categories = new SelectList(db.MenuCategories.Where(c => c.Enabled), "Id", "Name");
            return View();
        }

        ////只有後台能用
        //[HttpPost]
        //public ActionResult Create(Menu menu, HttpPostedFileBase imageUpload)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (imageUpload != null && imageUpload.ContentLength > 0)
        //        {
        //            // 獲取圖片的檔案名稱
        //            var fileName = Path.GetFileName(imageUpload.FileName);

        //            // 設定儲存路徑
        //            var path = Path.Combine(Server.MapPath("~/img"), fileName);

        //            // 將圖片儲存到伺服器
        //            imageUpload.SaveAs(path);

        //            // 將檔案名稱存入 Menu 的 FileName 屬性
        //            menu.FileName = fileName;
        //        }
        //        else
        //        {
        //            // 如果沒有上傳圖片，添加錯誤
        //            ModelState.AddModelError("FileName", "圖片是必填的");
        //        }

        //        menu.Enabled = true; // 預設設置為上架狀態
        //        // 設置時間戳
        //        menu.Createdtime = DateTime.Now;
        //        menu.Modifytime = DateTime.Now;

        //        // 保存菜單項目
        //        db.Menus.Add(menu);
        //        db.SaveChanges();

        //        return RedirectToAction("Index");
        //    }

        //    // 如果模型驗證失敗，重新傳遞分類列表
        //    ViewBag.Categories = new SelectList(db.MenuCategories.Where(c => c.Enabled), "Id", "Name");
        //    return View(menu);
        //}

        [HttpPost]
        public ActionResult Create(Menu menu, HttpPostedFileBase imageUpload)
        {
            if (ModelState.IsValid)
            {
                if (imageUpload != null && imageUpload.ContentLength > 0)
                {
                    {
                        string path = System.Configuration.ConfigurationManager.AppSettings["uploadPath"].ToString();
                        path = Path.Combine(path, "Products");

                        string storageSite = System.Configuration.ConfigurationManager.AppSettings["storageSite"].ToString();

                        if (ModelState.IsValid)
                        {
                            if (imageUpload != null)
                            {
                                menu.FileName = imageUpload.FileName;
                                imageUpload.SaveAs(Path.Combine(path, menu.FileName));
                            }
                            else
                            {
                                // 如果沒有上傳圖片，添加錯誤
                                ModelState.AddModelError("FileName", "圖片是必填的");
                            }
                            //// redirect 到指定的url
                            //var url = $"{storageSite}uploads/Products/{menu.FileName}";
                            //return Redirect(url);
                        }

                    }
                }
                menu.Enabled = true; // 預設設置為上架狀態
                // 設置時間戳
                menu.Createdtime = DateTime.Now;
                menu.Modifytime = DateTime.Now;

                // 保存菜單項目
                db.Menus.Add(menu);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            //如果模型驗證失敗，重新傳遞分類列表
            ViewBag.Categories = new SelectList(db.MenuCategories.Where(c => c.Enabled), "Id", "Name");
            return View(menu);
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);  // 如果 id 為 null，返回錯誤
            }

            var item = db.Menus.Find(id.Value);  // 確保使用 id.Value
            if (item == null)
            {
                return HttpNotFound();  // 如果找不到項目，返回 404 錯誤
            }

            // 傳遞數據到視圖
            ViewBag.Categories = new SelectList(db.MenuCategories.Where(c => c.Enabled), "Id", "Name", item.CategoryID);
            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(Menu menu, HttpPostedFileBase imageUpload)
        {
            if (ModelState.IsValid)
            {
                // 如果有上傳檔案
                if (imageUpload != null && imageUpload.ContentLength > 0)
                {
                    // 獲取檔案名稱
                    var fileName = Path.GetFileName(imageUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/img"), fileName);

                    // 儲存上傳的檔案
                    imageUpload.SaveAs(path);

                    // 將檔案名稱賦值給 FileName
                    menu.FileName = fileName;
                }
                else
                {
                    // 如果沒有上傳檔案，保持原有的 FileName
                    var existingMenu = db.Menus.AsNoTracking().FirstOrDefault(m => m.Id == menu.Id);
                    if (existingMenu != null)
                    {
                        menu.FileName = existingMenu.FileName;
                    }
                }

                // 設置時間戳
                menu.Createdtime = DateTime.Now;
                menu.Modifytime = DateTime.Now;

                db.Entry(menu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menu);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var menu = db.Menus.Find(id);
            if (menu == null)
            {
                return HttpNotFound();
            }

            db.Menus.Remove(menu);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: CreateCategory
        public ActionResult CreateCategory()
        {
            // 傳遞分類列表到視圖
            ViewBag.Categories = db.MenuCategories.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CreateCategory(MenuCategory category)
        {
            if (ModelState.IsValid)
            {
                category.Enabled = true;  // 設置分類為啟用
                db.MenuCategories.Add(category);
                db.SaveChanges();
                return RedirectToAction("CreateCategory");  // 保存後重新導向回到創建分類頁面
            }

            // 如果新增失敗，重新加載分類列表
            ViewBag.Categories = db.MenuCategories.ToList();
            return View(category);
        }

        // POST: DeleteCategory
        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
            // 找到對應的分類
            var category = db.MenuCategories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            // 檢查是否有任何產品與該分類相關聯
            var isCategoryInUse = db.Menus.Any(m => m.CategoryID == id);
            if (isCategoryInUse)
            {
                // 返回一個錯誤訊息，如果該分類下有產品
                ViewBag.ErrorMessage= "此分類下仍有產品，無法刪除。";
                ViewBag.Categories = db.MenuCategories.ToList();
                return View("CreateCategory");
            }
            else { 
            // 刪除分類
            db.MenuCategories.Remove(category);
            db.SaveChanges();

            return RedirectToAction("CreateCategory");}
        }

        // POST: ToggleCategoryStatus
        [HttpPost]
        public ActionResult ToggleCategoryStatus(int id)
        {
            var category = db.MenuCategories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            // 切換分類的啟用狀態
            category.Enabled = !category.Enabled;
            db.SaveChanges();

            return RedirectToAction("CreateCategory");
        }



    }
}