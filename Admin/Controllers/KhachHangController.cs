using DAL;
using Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class KhachHangController : Controller
    {
        // GET: KhachHang
        private DBConnect db = new DBConnect();
        public ActionResult Index(string searchString, int? page)
        {
            List<KhachHang> kh = new KhachHang_DAL().Select_KhachHang_All();
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(kh.OrderBy(p => p.Id).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Add(int? id)
        {
            var kh = new KhachHang();

            if (id != null)
            {
                var nv = db.KhachHangs.Find(id);
                return PartialView("Add", nv);
            }
            return PartialView("Add", kh);
        }

        [HttpPost]
        public JsonResult Add(KhachHang model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { code = 400, msg = "Dữ liệu không hợp lệ." }, JsonRequestBehavior.AllowGet);
                }

                model.CreatedDate = DateTime.Now;
                model.CreatedBy = Session["UserName"] != null ? Session["UserName"].ToString() : "Unknown";
                model.IsDeleted = false;

                var result = new KhachHang_DAL().Insert(model);
                if (result > 0)
                {
                    return Json(new { id = result, code = 200, msg = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Thêm mới thất bại" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return HttpNotFound();
            }
            KhachHang lstmodel = new KhachHang_DAL().SelectById(id);
            if (lstmodel == null)
            {
                return HttpNotFound();
            }

            return PartialView("Add", lstmodel);
        }
        [HttpPost]
        public JsonResult Update(KhachHang model)
        {
            try
            {
                model.UpdatedBy = Session["UserName"] != null ? Session["UserName"].ToString() : "Unknown";
                model.UpdatedDate = DateTime.Now;
                model.IsDeleted = false;

                var result = new KhachHang_DAL().Update(model);
                if (result > 0)
                {
                    return Json(new { code = 200, msg = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Cập nhật thất bại" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult DeleteAccount(int Id, string TenNguoiXoa)
        {
            try
            {
                TenNguoiXoa = Session["UserName"] != null ? Session["UserName"].ToString() : "Unknown";
                var result = new KhachHang_DAL().Delete(Id, TenNguoiXoa);
                if (result)
                {
                    return Json(new { code = 200, msg = "Xóa thành công" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Xóa thất bại" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = db.Products.Find(Convert.ToInt32(item));
                        db.Products.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public JsonResult ToggleStatus(int Id, string nguoiThucHien, string lyDo)
        {
            try
            {
                nguoiThucHien = Session["UserName"] != null ? Session["UserName"].ToString() : "Unknown";
                var result = new KhachHang_DAL().ToggleStatus(Id, nguoiThucHien, lyDo);
                if (result)
                {
                    return Json(new { code = 200, msg = "Cập nhật trạng thái thành công" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Cập nhật trạng thái thất bại" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.InnerException?.Message ?? ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}