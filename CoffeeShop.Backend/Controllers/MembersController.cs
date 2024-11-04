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
    [MyAuthorize(Roles = "4")]
    public class MembersController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Members
        public ActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var members = db.Members.AsQueryable(); // 使用 IQueryable 進行延遲加載

            // 根據搜索條件過濾會員
            if (!string.IsNullOrEmpty(searchString))
            {
                members = members.Where(m => m.Name.Contains(searchString) || m.Email.Contains(searchString));
            }

            // 計算總會員數和總頁數
            var totalMembers = members.Count();
            var totalPages = (int)Math.Ceiling(totalMembers / (double)pageSize);

            // 獲取當前頁的會員數據，先進行排序
            var memberViewModels = members
                .OrderBy(m => m.Name) // 根據姓名排序，可以根據你的需求選擇排序字段
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MemberViewModel
                {
                    Name = m.Name,
                    Email = m.Email,
                    Phone = m.Phone,
                    Birthday = m.Birthday.HasValue ? m.Birthday.Value : DateTime.MinValue,
                    Gender = m.Gender.HasValue ? m.Gender.Value : false
                }).ToList();

            // 將分頁信息傳遞給視圖
            ViewBag.SearchString = searchString; // 保存搜索字符串
            ViewBag.PageNumber = page;
            ViewBag.PageCount = totalPages;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;

            return View(memberViewModels);
        }

    }
}