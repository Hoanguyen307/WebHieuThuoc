using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class KhachHangController : Controller
    {
        // GET: KhachHang
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UpdateInfo()
        {
            var kh = Session["Login"] as KhachHang;
            if (kh == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateInfo(KhachHang model)
        {
            try
            {
                var khSession = Session["Login"] as KhachHang;
                if (khSession == null)
                    return Json(new { code = 401, msg = "Phiên đăng nhập hết hạn!" });

                model.Id = khSession.Id;
                model.UpdatedBy = "Khách hàng";

                var dal = new KhachHang_DAL();
                var rows = dal.UpdateInfo(model);

                if (rows > 0)
                {
                    khSession.FullName = model.FullName;
                    khSession.Gender = model.Gender;
                    khSession.BirthDate = model.BirthDate;
                    khSession.Phone = model.Phone;
                    khSession.Email = model.Email;
                    khSession.Address = model.Address;
                    Session["Login"] = khSession;

                    return Json(new { code = 200, msg = "Cập nhật thông tin thành công!" });
                }
                else
                {
                    return Json(new { code = 400, msg = "Không thể cập nhật thông tin." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi hệ thống: " + ex.Message });
            }
        }

        public ActionResult Diem()
        {
            var kh = Session["Login"] as KhachHang;
            if (kh == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var dal = new KhachHang_DAL();

            int tongDiem = dal.GetCustomerPoints(kh.Id);
            ViewBag.TongDiem = tongDiem;

            var lichSu = dal.LichSu_Diem(kh.Id);
            ViewBag.LichSuDiem = lichSu;

            return View();
        }

        public ActionResult DonHang()
        {
            var khachHang = Session["Login"] as KhachHang;
            if (khachHang == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orderDAL = new Order_DAL();
            var donHang = orderDAL.LichSu_DonHang(khachHang.Id);

            return View(donHang);
        }
    }
}