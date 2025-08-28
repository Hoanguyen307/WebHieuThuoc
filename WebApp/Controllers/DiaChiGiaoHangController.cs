using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class DiaChiGiaoHangController : Controller
    {
        private readonly DiaChiGiaoHang_DAL _dal = new DiaChiGiaoHang_DAL();
        // GET: DiaChiGiaoHang
        public ActionResult Index()
        {
            var kh = Session["Login"] as KhachHang;
            if (kh == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var list = _dal.GetByKhachHang(kh.Id);
            return View(list);
        }

        public ActionResult Create()
        {
            var kh = Session["Login"] as KhachHang;
            if (kh == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return PartialView("Create", new DiaChiGiaoHang());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(DiaChiGiaoHang model)
        {
            try
            {
                var kh = Session["Login"] as KhachHang;
                if (kh == null)
                    return Json(new { code = 401, msg = "Phiên đăng nhập hết hạn!" });

                model.KhachHangId = kh.Id;
                var newId = _dal.Insert(model);

                if (newId > 0)
                    return Json(new { code = 200, msg = "Thêm địa chỉ thành công!" });
                else
                    return Json(new { code = 400, msg = "Không thể thêm địa chỉ." });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi hệ thống: " + ex.Message });
            }
        }

        public ActionResult Edit(int id)
        {
            var kh = Session["Login"] as KhachHang;
            if (kh == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var list = _dal.GetByKhachHang(kh.Id);
            var model = list.FirstOrDefault(x => x.DiaChiId == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return PartialView("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(DiaChiGiaoHang model)
        {
            try
            {
                var kh = Session["Login"] as KhachHang;
                if (kh == null)
                    return Json(new { code = 401, msg = "Phiên đăng nhập hết hạn!" });

                model.KhachHangId = kh.Id;
                var ok = _dal.Update(model);

                if (ok)
                    return Json(new { code = 200, msg = "Cập nhật địa chỉ thành công!" });
                else
                    return Json(new { code = 400, msg = "Không thể cập nhật địa chỉ." });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var kh = Session["Login"] as KhachHang;
                if (kh == null)
                    return Json(new { code = 401, msg = "Phiên đăng nhập hết hạn!" });

                var ok = _dal.Delete(id, kh.Id);

                if (ok)
                    return Json(new { code = 200, msg = "Xóa địa chỉ thành công!" });
                else
                    return Json(new { code = 400, msg = "Không thể xóa địa chỉ." });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SetDefault(int id)
        {
            try
            {
                var kh = Session["Login"] as KhachHang;
                if (kh == null)
                    return Json(new { code = 401, msg = "Phiên đăng nhập hết hạn!" });

                var ok = _dal.SetDefault(kh.Id, id);

                if (ok)
                    return Json(new { code = 200, msg = "Đặt địa chỉ mặc định thành công!" });
                else
                    return Json(new { code = 400, msg = "Không thể đặt địa chỉ mặc định." });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}