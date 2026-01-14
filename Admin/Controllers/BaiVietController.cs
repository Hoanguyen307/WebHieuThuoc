using DAL;
using Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Models.Post;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Configuration;

namespace Admin.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class BaiVietController : Controller
    {
        // GET: Admin/BaiViet
        private DBConnect db = new DBConnect(); 
        private readonly Cloudinary _cloudinary;

        public BaiVietController()
        {
            var account = new Account(
                ConfigurationManager.AppSettings["Cloudinary_CloudName"],
                ConfigurationManager.AppSettings["Cloudinary_ApiKey"],
                ConfigurationManager.AppSettings["Cloudinary_ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

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
        [ValidateInput(false)]
        public JsonResult Add(Post model, HttpPostedFileBase ImageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                                   .Select(x => new { key = x.Key, msg = x.Value.Errors[0].ErrorMessage });
                    return Json(new { code = 400, msg = "Dữ liệu không hợp lệ", errors = errors });
                }
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    model.AnhDaiDien = UploadToCloud(ImageFile);
                }

                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User?.Identity?.Name ?? "Unknown";

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
        [ValidateInput(false)]
        public JsonResult Update(Post model, HttpPostedFileBase ImageFile)
        {
            try
            {
                model.UpdatedBy = User?.Identity?.Name ?? "Unknown";
                model.UpdatedDate = DateTime.Now;

                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    model.AnhDaiDien = UploadToCloud(ImageFile);
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
                TenNguoiXoa = User?.Identity?.Name ?? "Unknown";
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
        public JsonResult ToggleStatus(int Id)
        {
            try
            {
                var result = new BaiViet_DAL().ToggleStatus(Id);
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

        private string UploadToCloud(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0) return null;

            try
            {
                string fileNameWithExtension = Path.GetFileName(file.FileName);
                string fileNameOnly = Path.GetFileNameWithoutExtension(file.FileName);

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileNameWithExtension, file.InputStream),
                    PublicId = fileNameOnly,
                    Folder = "NhaThuoc/BaiViet",
                    Overwrite = true,
                    UseFilename = true,
                    UniqueFilename = false
                };

                var result = _cloudinary.Upload(uploadParams);

                if (result.Error != null)
                {
                    System.Diagnostics.Debug.WriteLine("Lỗi: " + result.Error.Message);
                    return null;
                }

                return result.SecureUrl?.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}