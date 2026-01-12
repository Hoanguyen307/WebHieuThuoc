using DAL;
using Models;
using Models.LichLamViecViewModel;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Models.KhachHang;
using static Models.LichLamViec;
using static Models.NhanVien;

namespace Admin.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class NhanVienController : Controller
    {
        // GET: Admin/NhanVien
        private DBConnect db = new DBConnect();

        public ActionResult Index(string searchString, int? Month, int? Year, int? PositionId, int? ShiftId)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var year = Enumerable.Range(currentYear - 5, 11).Select(y => new { Id = y, Name = y.ToString() }).ToList();
            ViewBag.Years = new SelectList(year, "Id", "Name", Year);

            var months = Enumerable.Range(1, 12).Select(m => new { Id = m, Name = $"Tháng {m}" }).ToList();
            ViewBag.Months = new SelectList(months, "Id", "Name", Month);

            var listPosition = new NhanVien_DAL().Select_Position_All();
            ViewBag.Positions = new SelectList(listPosition, "Id", "Name", PositionId);

            var listCaLam = new NhanVien_DAL().Select_CaLam_All();
            ViewBag.Calams = new SelectList(listCaLam, "Id", "Name", ShiftId);

            return View();
        }

        [HttpGet]
        public JsonResult GetNhanVien(string searchString, int? Month, int? Year, int? PositionId, int? ShiftId, int page = 1, int pageSize = 10)
        {
            if (Month == 0) Month = null;
            if (Year == 0) Year = null;

            NhanVienFilter filter = new NhanVienFilter
            {
                FullName = string.IsNullOrWhiteSpace(searchString) ? null : searchString,
                Month = Month,
                Year = Year,
                PositionId = PositionId,
                ShiftId = ShiftId,
            };

            var processes = new NhanVien_DAL().Select_NhanVien_All(filter);
            var pagedList = processes.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);

            return Json(new
            {
                items = pagedList.ToList(),
                totalCount = pagedList.TotalItemCount,
                currentPage = pagedList.PageNumber,
                pageSize = pagedList.PageSize
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateTime(LichLamViec model)
        {
            try
            {
                model.UpdatedBy = User?.Identity?.Name ?? "Unknown";
                model.UpdatedDate = DateTime.Now;
                model.IsDeleted = false;

                var result = new NhanVien_DAL().UpdateTime(model);
                if (result > 0)
                {
                    return Json(new { code = 200, msg = "Cập nhật thành công", icon = "success" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Cập nhật thất bại", icon = "warning" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message, icon = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateCalam(LichLamViec model)
        {
            try
            {
                var listCaLam = new NhanVien_DAL().Select_CaLam_All();
                ViewBag.Calams = new SelectList(listCaLam, "Id", "Name", model.ShiftId);

                model.UpdatedBy = User?.Identity?.Name ?? "Unknown";
                model.UpdatedDate = DateTime.Now;
                model.IsDeleted = false;

                var result = new NhanVien_DAL().UpdateCaLam(model);
                if (result > 0)
                {
                    return Json(new { code = 200, msg = "Cập nhật thành công", icon = "success" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Cập nhật thất bại", icon = "warning" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message, icon = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ChiTietNhanVien(int id)
        {
            try
            {
                var nhanvien = new NhanVien_DAL().SelectById(id);
                if (nhanvien != null)
                {
                    return Json(new { success = true, data = nhanvien }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Không tìm thấy nhân viên" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Add(int? id)
        {
            var nhanvien = new NhanVien();
            var listCaLam = new NhanVien_DAL().Select_CaLam_All();
            ViewBag.Calams = new SelectList(listCaLam, "Id", "Name");

            var listPosition = new NhanVien_DAL().Select_Position_All();
            ViewBag.Positions = new SelectList(listPosition, "Id", "Name");

            if (id != null)
            {
                var nv = db.NhanViens.Find(id);
                return PartialView("Add", nv);
            }
            return PartialView("Add", nhanvien);
        }

        [HttpPost]
        public JsonResult Add(NhanVien model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { code = 400, msg = "Dữ liệu không hợp lệ." }, JsonRequestBehavior.AllowGet);
                }

                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User?.Identity?.Name ?? "Unknown";

                var result = new NhanVien_DAL().Insert(model);
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
            NhanVien lstmodel = new NhanVien_DAL().SelectById(id);
            if (lstmodel == null)
            {
                return HttpNotFound();
            }
            var listCaLam = new NhanVien_DAL().Select_CaLam_All();
            ViewBag.Calams = new SelectList(listCaLam, "Id", "Name");

            var listPosition = new NhanVien_DAL().Select_Position_All();
            ViewBag.Positions = new SelectList(listPosition, "Id", "Name");

            return PartialView("Add", lstmodel);
        }
        [HttpPost]
        public JsonResult Update(NhanVien model)
        {
            try
            {
                model.UpdatedBy = User?.Identity?.Name ?? "Unknown";
                model.UpdatedDate = DateTime.Now;

                var result = new NhanVien_DAL().Update(model);
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
                var result = new NhanVien_DAL().Delete(Id, TenNguoiXoa);
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
        public ActionResult IsActive(int ID)
        {
            var item = db.Products.Find(ID);
            if (item != null)
            {
                item.KichHoat = !item.KichHoat;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isActive = item.KichHoat });
            }
            return Json(new { success = false });
        }

    }
}