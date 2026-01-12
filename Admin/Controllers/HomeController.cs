using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    /*[Authorize(Roles = "Admin, Employee")]*/
    public class HomeController : Controller
    {
        private readonly DBConnect db = new DBConnect();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult LayThongKeTongQuan(DateTime? tuNgay, DateTime? denNgay)
        {
            try
            {
                var thongKe = new Dashboard_DAL().LayThongKeTongQuan(tuNgay, denNgay);
                if (thongKe != null)
                {
                    return Json(new
                    {
                        code = 200,
                        data = thongKe
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Không có dữ liệu" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult LayDuLieuBanHang(string kyHan = "ngay", DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            try
            {
                var duLieu = new Dashboard_DAL().LayDuLieuBanHang(kyHan, tuNgay, denNgay);
                if (duLieu != null)
                {
                    return Json(new
                    {
                        code = 200,
                        data = duLieu
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Không có dữ liệu bán hàng" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult LayTongKetTaiChinh(DateTime? tuNgay, DateTime? denNgay)
        {
            try
            {
                var taiChinh = new Dashboard_DAL().LayTongKetTaiChinh(tuNgay, denNgay);
                if (taiChinh != null)
                {
                    return Json(new
                    {
                        code = 200,
                        data = taiChinh
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Không có dữ liệu tài chính" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}