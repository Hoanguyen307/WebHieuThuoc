using DAL;
using Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static Models.DichVu;

namespace AdminPhongKham.Controllers
{
    public class DichVuController : Controller
    {
        private DBConnect db = new DBConnect();

        public ActionResult Index(string searchString, decimal? MinPrice, decimal? MaxPrice, int? DanhMucId)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            //var listCategory = new DichVuPhongKham_DAL().Select_All();
            //ViewBag.DichVuPhongKhams = new SelectList(listCategory, "Id", "TenDichVu", DanhMucId);


            ViewBag.MinPrice = MinPrice;
            ViewBag.MaxPrice = MaxPrice;

            return View();
        }
        [HttpGet]
        public JsonResult GetDichVu(string searchString, decimal? MinPrice, decimal? MaxPrice, int page = 1, int pageSize = 10)
        {

            DichVuFilter filter = new DichVuFilter
            {
                Name = string.IsNullOrWhiteSpace(searchString) ? null : searchString,
                MinPrice = MinPrice,
                MaxPrice = MaxPrice
            };

            var processes = new DichVu_DAL().Select_All(filter);
            int threshold = 10;

            var pagedList = processes.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);
            var result = pagedList.Select(p => new
            {
                p.Id,
                p.HinhAnh,
                p.TenDichVu,
                p.TenDanhMuc,
                p.Gia,
                p.ThoiGian,
                p.IsActive
            }).ToList();

            return Json(new
            {
                items = result,
                totalCount = pagedList.TotalItemCount,
                currentPage = pagedList.PageNumber,
                pageSize = pagedList.PageSize
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add(int? id)
        {
            var dichvu = new DichVu();
            var listCategory = new DichVuPhongKham_DAL().Select_All();
            ViewBag.DichVuPhongKhams = new SelectList(listCategory, "DichVuId", "TenDichVu");


            if (id != null)
            {
                var dv = db.DichVus.Find(id);
                return PartialView(dv);
            }
            return PartialView("Add", dichvu);
        }
        [HttpPost]
        public JsonResult Add(DichVu model, HttpPostedFileBase ImageFile)
        {
            try
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads/DichVu/"), fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    ImageFile.SaveAs(path);
                    model.HinhAnh = "/Uploads/DichVu/" + fileName;
                }

                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User?.Identity?.Name ?? "Unknown";
                model.IsDeleted = false;

                var result = new DichVu_DAL().Insert(model);
                if (result > 0)
                {
                    return Json(new { id = result, code = 200, msg = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Thêm mới thất bại" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi:" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return HttpNotFound();
            }
            DichVu lstmodel = new DichVu_DAL().SelectById(id);
            if (lstmodel == null)
            {
                return HttpNotFound();
            }
            var listCategory = new DichVuPhongKham_DAL().Select_All();
            ViewBag.DichVuPhongKhams = new SelectList(listCategory, "DichVuId", "TenDichVu");

            return PartialView("Add", lstmodel);
        }
        [HttpPost]
        public JsonResult Update(DichVu model, HttpPostedFileBase ImageFile)
        {
            try
            {
                model.UpdatedBy = User?.Identity?.Name ?? "Unknown";
                model.UpdatedDate = DateTime.Now;
                model.IsDeleted = false;

                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads/DichVu/"), fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    ImageFile.SaveAs(path);

                    model.HinhAnh = "/Uploads/DichVu/" + fileName;
                }

                var result = new DichVu_DAL().Update(model);
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
                var result = new DichVu_DAL().Delete(Id, TenNguoiXoa);
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
        public JsonResult ToggleHienThi(int Id, bool isActive)
        {
            try
            {
                var result = new DichVu_DAL().Update_IsActive(Id, isActive);
                if (result)
                {
                    return Json(new { code = 200, msg = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Cập nhật thất bại" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 550, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}