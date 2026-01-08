using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class BaiVietController : Controller
    {
        // GET: BaiViet
        public ActionResult Index(string category = "", int page = 1)
        {
            int pageSize = 9;
            int totalCount = 0;
            var data = new BaiViet_DAL().Select_ForUser(category, page, pageSize, out totalCount);

            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = page;
            ViewBag.Category = category;

            return View(data);
        }

        public JsonResult GetBlogData(string category = "", int page = 1)
        {
            int pageSize = 9;
            int totalCount = 0;
            var data = new BaiViet_DAL().Select_ForUser(category, page, pageSize, out totalCount);

            string html = RenderRazorViewToString("_BlogListPartial", data);

            return Json(new
            {
                html = html,
                totalCount = totalCount,
                page = page
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LatestBlogPartial()
        {
            var data = new BaiViet_DAL().SelectTopNew(3);
            return PartialView("_LatestBlogPartial", data);
        }
        public ActionResult Details(int id)
        {
            var model = new BaiViet_DAL().SelectById(id);
            if (model == null) return HttpNotFound();

            return View(model);
        }
        protected string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}