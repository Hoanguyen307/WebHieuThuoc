using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class DichVuController : Controller
    {
        DichVuPhongKham_DAL _dal = new DichVuPhongKham_DAL();
        // GET: DichVu
        public ActionResult Index(string sortOrder = "newest", int pageSize = 40)
        {
            var list = _dal.Select_All();
            var category = _dal.Select_All();
            ViewBag.DichVuPhongKhams = category;
            return View(list);
        }
    }
}