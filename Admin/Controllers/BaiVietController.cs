using DAL;
using Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Models.NhanVien;
using static Models.Post;

namespace Admin.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class BaiVietController : Controller
    {
        // GET: Admin/BaiViet
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
        public JsonResult GetBaiViet(string searchString, int? Month, int? Year, int? PositionId, int? ShiftId, int page = 1, int pageSize = 10)
        {
            if (Month == 0) Month = null;
            if (Year == 0) Year = null;

            PostFilter filter = new PostFilter
            {
                TieuDe = string.IsNullOrWhiteSpace(searchString) ? null : searchString,
                Month = Month,
                Year = Year
            };

            var processes = new BaiViet_DAL().Select_BaiViet_All(filter);
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
            var baiviet = new Post();

            if (id != null)
            {
                var bv = db.BaiViets.Find(id);
                return PartialView("Add", bv);
            }
            return PartialView("Add", baiviet);
        }

        [HttpPost]
        public JsonResult Add(Post model, HttpPostedFileBase ImageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { code = 400, msg = "Dữ liệu không hợp lệ." }, JsonRequestBehavior.AllowGet);
                }
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads/BaiViet/"), fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    ImageFile.SaveAs(path);
                    model.AnhDaiDien = "/Uploads/BaiViet/" + fileName;
                }

                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User?.Identity?.Name ?? "Unknown";
                model.IsDeleted = false;

                var result = new BaiViet_DAL().Insert(model);
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
            Post lstmodel = new BaiViet_DAL().SelectById(id);
            if (lstmodel == null)
            {
                return HttpNotFound();
            }

            return PartialView("Add", lstmodel);
        }
        [HttpPost]
        public JsonResult Update(Post model, HttpPostedFileBase ImageFile)
        {
            try
            {
                model.UpdatedBy = User?.Identity?.Name ?? "Unknown";
                model.UpdatedDate = DateTime.Now;
                model.IsDeleted = false;

                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads/Category/"), fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    ImageFile.SaveAs(path);

                    model.AnhDaiDien = "/Uploads/Category/" + fileName;
                }

                var result = new BaiViet_DAL().Update(model);
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
                var result = new BaiViet_DAL().Delete(Id, TenNguoiXoa);
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
                        var obj = db.BaiViets.Find(Convert.ToInt32(item));
                        db.BaiViets.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public JsonResult ToggleStatus(int Id, string nguoiThucHien)
        {
            try
            {
                nguoiThucHien = Session["UserName"] != null ? Session["UserName"].ToString() : "Unknown";
                var result = new BaiViet_DAL().ToggleStatus(Id, nguoiThucHien);
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