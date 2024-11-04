using CoffeeShop.Backend.Models.Components;
using CoffeeShop.Backend.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CoffeeShop.Backend.Controllers
{
    [MyAuthorize(Roles = "2")]
    public class OrdersController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private readonly HttpClient _httpClient;
        public OrdersController()
        {
            // 設定 HttpClient 來調用第三個專案的 API
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7060/api/") // 確保使用正確的 API URL
            };
        }

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

        [HttpPost]
        public async Task<ActionResult> Create(Menu menu, HttpPostedFileBase imageUpload)
        {
            if (ModelState.IsValid)
            {
                if (imageUpload != null && imageUpload.ContentLength > 0)
                {
                    // 上傳照片到第三個專案的 API
                    var content = new MultipartFormDataContent();
                    var fileContent = new StreamContent(imageUpload.InputStream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageUpload.ContentType);
                    content.Add(fileContent, "imageUpload", imageUpload.FileName);

                    var response = await _httpClient.PostAsync("photo", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var imageFileName = await response.Content.ReadAsStringAsync();
                        menu.FileName = imageFileName; // 保存文件名
                    }
                    else
                    {
                        ModelState.AddModelError("FileName", "圖片上傳失敗");
                    }
                }
                else
                {
                    ModelState.AddModelError("FileName", "圖片是必填的");
                }

                menu.Enabled = true; // 預設設置為上架狀態
                menu.Createdtime = DateTime.Now;
                menu.Modifytime = DateTime.Now;

                db.Menus.Add(menu);
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(db.MenuCategories.Where(c => c.Enabled), "Id", "Name");
            return View(menu);
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var item = db.Menus.Find(id.Value);
            if (item == null)
            {
                return HttpNotFound();
            }

            ViewBag.Categories = new SelectList(db.MenuCategories.Where(c => c.Enabled), "Id", "Name", item.CategoryID);
            return View(item);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Menu menu, HttpPostedFileBase imageUpload)
        {
            if (ModelState.IsValid)
            {
                // 獲取舊的照片名稱
                var existingMenu = await db.Menus.AsNoTracking().FirstOrDefaultAsync(m => m.Id == menu.Id);
                string oldFileName = existingMenu?.FileName;

                if (imageUpload != null && imageUpload.ContentLength > 0)
                {
                    // 上傳照片到第三個專案的 API
                    var content = new MultipartFormDataContent();
                    var fileContent = new StreamContent(imageUpload.InputStream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageUpload.ContentType);
                    content.Add(fileContent, "imageUpload", imageUpload.FileName);

                    var response = await _httpClient.PostAsync("photo", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var imageFileName = await response.Content.ReadAsStringAsync();
                        menu.FileName = imageFileName; // 保存新文件名

                        // 刪除舊照片
                        if (!string.IsNullOrEmpty(oldFileName))
                        {
                            var deleteResponse = await _httpClient.DeleteAsync($"photo/{oldFileName}");
                            if (!deleteResponse.IsSuccessStatusCode)
                            {
                                ModelState.AddModelError("FileName", "舊照片刪除失敗");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("FileName", "圖片上傳失敗");
                    }
                }
                else
                {
                    // 如果沒有上傳新照片，保持舊的文件名
                    menu.FileName = oldFileName;
                }

                menu.Createdtime = DateTime.Now;
                menu.Modifytime = DateTime.Now;

                db.Entry(menu).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(menu);
        }
        public async Task<ActionResult> Delete(int id)
        {
            var menu = await db.Menus.FindAsync(id);
            if (menu == null)
            {
                return HttpNotFound();
            }

            // 刪除菜單前，調用第三個專案的 API 刪除對應照片
            var response = await _httpClient.DeleteAsync($"photo/{menu.FileName}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "刪除圖片失敗");
            }

            db.Menus.Remove(menu);
            await db.SaveChangesAsync();
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