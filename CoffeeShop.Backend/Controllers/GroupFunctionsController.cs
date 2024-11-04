using CoffeeShop.Backend.Models.Components;
using CoffeeShop.Backend.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoffeeShop.Backend.Controllers
{
    [MyAuthorize(Roles = "4")]
    public class GroupFunctionsController : Controller
    {
        private readonly GroupFunctionService _groupFunctionService;

        public GroupFunctionsController()
        {
            _groupFunctionService = new GroupFunctionService();
        }


        public ActionResult Index()
        {
            var customPrincipal = (CustomPrincipal)User;
            int modifyId = customPrincipal.Id;


            _groupFunctionService.SyncGroupFunctions(modifyId);


            var groups = _groupFunctionService.GetGroups();


            ViewBag.Groups = groups;
            ViewBag.SelectedGroupName = "請選擇群組";

            return View();
        }

        [HttpGet]
        public JsonResult GetFunctionsByGroup(int groupId)

        {

            var functions = _groupFunctionService.GetFunctionsByGroup(groupId);
            return Json(functions, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdatePermissions(int groupId, int[] selectedFunctions)
        {
            var customPrincipal = (CustomPrincipal)User;
            int modifyId = customPrincipal.Id;

            _groupFunctionService.UpdateGroupFunctions(groupId, selectedFunctions, modifyId);

            return RedirectToAction("Index");
        }


    }
}