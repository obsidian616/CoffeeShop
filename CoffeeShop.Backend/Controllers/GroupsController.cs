using CoffeeShop.Backend.Models.Components;
using CoffeeShop.Backend.Models.Repositories;
using CoffeeShop.Backend.Models.Services;
using CoffeeShop.Backend.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoffeeShop.Backend.Controllers
{
    [MyAuthorize(Roles = "4")]
    public class GroupsController : Controller
    {
        private readonly GroupService _groupService;

        public GroupsController()
        {
            _groupService = new GroupService(new GroupRepository());
        }

        // GET: Groups
        public ActionResult Index()
        {
            var groups = _groupService.GetAllGroups();
            var groupVms = groups.Select(g => new GroupVm
            {
                Id = g.Id,
                GroupName = g.GroupName,
                Enabled = g.Enabled
            }).ToList();

            return View(groupVms);
        }

        // GET: Groups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        [HttpPost]
        public ActionResult Create(GroupVm model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "無效的輸入資料", errors });
            }

            var customPrincipal = (CustomPrincipal)User;
            int modifyId = customPrincipal.Id;
            var result = _groupService.AddGroup(model, modifyId);
            if (result.IsSuccess)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = result.ErrorMessage });
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(int id)
        {
            var group = _groupService.GetGroupById(id);
            if (group == null)
            {
                return Json(new { success = false, message = "群組不存在" }, JsonRequestBehavior.AllowGet);
            }

            var groupVm = new GroupVm
            {
                Id = group.Id,
                GroupName = group.GroupName,
                Enabled = group.Enabled
            };

            return Json(new { success = true, group = groupVm }, JsonRequestBehavior.AllowGet);
        }

        // POST: Groups/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, GroupVm model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errors });
            }

            var customPrincipal = (CustomPrincipal)User;
            int modifyId = customPrincipal.Id;
            var result = _groupService.UpdateGroup(id, model, modifyId);
            if (result.IsSuccess)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = result.ErrorMessage });
        }

        // POST: Groups/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
          
            var result = _groupService.DeleteGroup(id);

          
            if (result.IsSuccess)
            {
                return Json(new { success = true });
            }


            return Json(new { success = false, message = result.ErrorMessage });
        }
    }
}
