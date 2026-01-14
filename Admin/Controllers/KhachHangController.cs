using DAL;
using Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Models.KhachHang;
using static Models.Product;

namespace Admin.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class KhachHangController : Controller
    {
        // GET: KhachHang
        private DBConnect db = new DBConnect();
        public ActionResult Index(string searchString, int? Month, int? Year)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var year = Enumerable.Range(currentYear - 5, 11).Select(y => new { Id = y, Name = y.ToString() }).ToList();
            ViewBag.Years = new SelectList(year, "Id", "Name", Year);

            var months = Enumerable.Range(1, 12).Select(m => new { Id = m, Name = $"Tháng {m}" }).ToList();
            ViewBag.Months = new SelectList(months, "Id", "Name", Month);

            return View();
        }

        [HttpGet]
        public JsonResult GetKhachHang(string searchString, int? Month, int? Year, int page = 1, int pageSize = 10)
        {
            if (Month == 0) Month = null;
            if (Year == 0) Year = null;

            KhachHangFilter filter = new KhachHangFilter
            {
                FullName = string.IsNullOrWhiteSpace(searchString) ? null : searchString,
                Month = Month,
                Year = Year
            };

            var processes = new KhachHang_DAL().Select_KhachHang_All(filter);
            var pagedList = processes.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);

            return Json(new
            {
                items = pagedList.ToList(),
                totalCount = pagedList.TotalItemCount,
                currentPage = pagedList.PageNumber,
                pageSize = pagedList.PageSize
            }, JsonRequestBehavior.AllowGet);
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
                model.CreatedBy = User?.Identity?.Name ?? "Unknown";
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
                model.UpdatedBy = User?.Identity?.Name ?? "Unknown";
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
                TenNguoiXoa = User?.Identity?.Name ?? "Unknown";
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
                nguoiThucHien = User?.Identity?.Name ?? "Unknown";
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
        [HttpGet]
        public JsonResult GetPointHistory(int customerId, int page = 1, int pageSize = 10)
        {
            try
            {
                var history = new KhachHang_DAL().LichSu_Diem(customerId);
                if (history == null || !history.Any())
                {
                    return Json(new { code = 404, msg = "Không có lịch sử điểm nào" }, JsonRequestBehavior.AllowGet);
                }
                var pagedList = history.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);
                return Json(new
                {
                    code = 200,
                    items = pagedList.ToList(),
                    totalCount = pagedList.TotalItemCount,
                    currentPage = pagedList.PageNumber,
                    pageSize = pagedList.PageSize
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}