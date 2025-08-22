using DAL;
using Models;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Models.Post;


namespace Admin.Controllers
{
    /*[Authorize(Roles = "Admin, Employee")]*/
    public class NhapKhoController : Controller
    {
        // GET: NhapKho
        private DBConnect db = new DBConnect();

        public ActionResult Index(int? Month, int? Year, string searchString)
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
        public JsonResult GetNhapKho( int? Month, int? Year, string searchString, int page = 1, int pageSize = 10)
        {
            if (Month == 0) Month = null;
            if (Year == 0) Year = null;

            NhapKhoFilter filter = new NhapKhoFilter
            {
                MaPhieu = string.IsNullOrWhiteSpace(searchString) ? null : searchString,
                Month = Month,
                Year = Year
            };

            var nhapkhos = new NhapKho_DAL().Select_NhapKho_All(filter);
            var pagedList = nhapkhos.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);

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
            var products = db.Products
                    .Select(p => new { p.Id, p.Name })
                    .ToList();

            ViewBag.Products = JsonConvert.SerializeObject(products);

            var nhapkho = new NhapKho();

            if (id != null)
            {
                var nk = db.NhapKhos.Find(id);
                return PartialView("Add", nk);
            }
            return PartialView("Add", nhapkho);
        }

        [HttpPost]
        public JsonResult Add(NhapKho model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { code = 400, msg = "Dữ liệu không hợp lệ." }, JsonRequestBehavior.AllowGet);
                }
                if (model.ChiTietNhapKho != null)
                {
                    model.TotalAmount = model.ChiTietNhapKho.Sum(p => p.SoLuong * p.DonGiaNhap);
                }
                else
                {
                    model.TotalAmount = 0;
                }
                model.CreatedDate = DateTime.Now;
                model.NgayNhap = DateTime.Now;
                model.CreatedBy = User?.Identity?.Name ?? "Unknown";
                model.IsDeleted = false;

                var result = new NhapKho_DAL().Insert(model);
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


        public ActionResult Edit(int nhapKhoId)
        {
            if (nhapKhoId <= 0)
            {
                return HttpNotFound();
            }
            var products = db.Products
                    .Select(p => new { p.Id, p.Name })
                    .ToList();

            ViewBag.Products = JsonConvert.SerializeObject(products);

            NhapKho lstmodel = new NhapKho_DAL().SelectById(nhapKhoId);
            if (lstmodel == null)
            {
                return HttpNotFound();
            }
            return PartialView("Add", lstmodel);
        }
        [HttpPost]
        public JsonResult Update(NhapKho model)
        {
            try
            {
                var oldData = new NhapKho_DAL().SelectById(model.Id);
                if (oldData == null)
                {
                    return Json(new { code = 404, msg = "Không tìm thấy phiếu nhập" });
                }

                model.NgayNhap = oldData.NgayNhap;

                if (model.ChiTietNhapKho != null)
                {
                    model.TotalAmount = model.ChiTietNhapKho.Sum(p => p.SoLuong * p.DonGiaNhap);
                }
                else
                {
                    model.TotalAmount = 0;
                }

                model.UpdatedBy = User?.Identity?.Name ?? "Unknown";
                model.UpdatedDate = DateTime.Now;
                model.IsDeleted = false;

                var result = new NhapKho_DAL().Update(model);

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
        public JsonResult DeleteAccount(int Id, string DeletedBy)
        {
            try
            {
                DeletedBy = User?.Identity?.Name ?? "Unknown";
                var result = new NhapKho_DAL().Delete(Id, DeletedBy);
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

        public ActionResult Detail(int nhapKhoId)
        {
            if (nhapKhoId <= 0)
            {
                return HttpNotFound();
            }
            var products = db.Products
                    .Select(p => new { p.Id, p.Name })
                    .ToList();

            ViewBag.Products = JsonConvert.SerializeObject(products);

            NhapKho lstmodel = new NhapKho_DAL().SelectById(nhapKhoId);
            if (lstmodel == null)
            {
                return HttpNotFound();
            }
            return PartialView("Detail", lstmodel);
        }
    }
}