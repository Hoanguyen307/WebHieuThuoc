using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Models.Post;

namespace Admin.Controllers
{
    public class DoanhThuController : Controller
    {
        private DBConnect db = new DBConnect();
        // GET: DoanhThu
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetBaoCaoDoanhThu(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var result = new DoanhThu_DAL().GetBaoCaoDoanhThu(fromDate, toDate);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}