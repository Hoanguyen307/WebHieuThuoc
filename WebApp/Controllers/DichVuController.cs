using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Models.DichVu;
using static Models.Product;

namespace WebApp.Controllers
{
    public class DichVuController : Controller
    {
        // GET: DichVu
        public ActionResult Index(int? DanhMucId, string sortOrder = "newest", int pageSize = 40)
        {
            var filter = new DichVuFilter
            {
                DanhMucId = DanhMucId
            };

            var dichvu = new DichVu_DAL().Select_Published(filter, sortOrder).Take(pageSize).ToList();
            var category = new DichVuPhongKham_DAL().Select_All();
            ViewBag.DichVuPhongKhams = category;

            ViewBag.SortOrder = sortOrder;
            ViewBag.SelectedCategoryId = DanhMucId;
            return View(dichvu);
        }
    }
}