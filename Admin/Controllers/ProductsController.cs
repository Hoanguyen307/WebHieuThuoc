using DAL;
using Models;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static Models.Product;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Configuration;

namespace Admin.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class ProductsController : Controller
    {
        private DBConnect db = new DBConnect(); 
        private readonly Cloudinary _cloudinary;
        public ProductsController()
        {
            var account = new Account(
                ConfigurationManager.AppSettings["Cloudinary_CloudName"],
                ConfigurationManager.AppSettings["Cloudinary_ApiKey"],
                ConfigurationManager.AppSettings["Cloudinary_ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }
        public ActionResult Index(string searchString, decimal? MinPrice, decimal? MaxPrice, int? ProductCategoryId, int? NhaCungCapId, int? Month, int? Year)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var year = Enumerable.Range(currentYear - 5, 11).Select(y => new { Id = y, Name = y.ToString() }).ToList();
            ViewBag.Years = new SelectList(year, "Id", "Name", Year);

            var months = Enumerable.Range(1, 12).Select(m => new { Id = m, Name = $"Tháng {m}" }).ToList();
            ViewBag.Months = new SelectList(months, "Id", "Name", Month);

            var listCategory = new ProductCategory_DAL().Select_Category_All();
            ViewBag.ProductCategories = new SelectList(listCategory, "DanhMucId", "TenDanhMuc", ProductCategoryId);

            var listNhaCungCap = new Product_DAL().Select_NhaCungCap_All();
            ViewBag.NhaCungCaps = new SelectList(listNhaCungCap, "NhaCungCapId", "TenNhaCungCap", NhaCungCapId);

            ViewBag.MinPrice = MinPrice;
            ViewBag.MaxPrice = MaxPrice;

            return View();
        }
        [HttpGet]
        public JsonResult GetProduct(string searchString, decimal? MinPrice, decimal? MaxPrice, int? ProductCategoryId, int? Month, int? Year, int page = 1, int pageSize = 10)
        {
            if (Month == 0) Month = null;
            if (Year == 0) Year = null;

            ProductFilter filter = new ProductFilter
            {
                Name = string.IsNullOrWhiteSpace(searchString) ? null : searchString,
                ProductCategoryId = ProductCategoryId,
                Month = Month,
                Year = Year
            };

            var processes = new Product_DAL().Select_Product_All(filter);
            int threshold = 10;

            var pagedList = processes.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);
            var result = pagedList.Select(p => new
            {
                p.ThuocId,
                p.HinhAnh,
                p.TenThuoc,
                p.ProductCategoryName,
                p.GiaGoc,
                p.GiaBan,
                p.SoLuong,
                p.DaBan,
                p.DonViTinh,
                p.TenNhaCungCap,
                p.KichHoat,
                CanhBaoHetHang = p.SoLuong <= threshold
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
            var product = new Product();
            var listCategory = new ProductCategory_DAL().Select_Category_All();
            ViewBag.ProductCategories = new SelectList(listCategory, "DanhMucId", "TenDanhMuc");

            var listNhaCungCap = new Product_DAL().Select_NhaCungCap_All();
            ViewBag.NhaCungCaps = new SelectList(listNhaCungCap, "NhaCungCapId", "TenNhaCungCap");
            if (id != null)
            {
                var sanpham = db.Products.Find();
                return PartialView(sanpham);
            }
            return PartialView("Add", product);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Add(Product model, HttpPostedFileBase ImageFile)
        {
            try
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    model.HinhAnh = UploadFileToCloud(ImageFile);
                }

                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User?.Identity?.Name ?? "Unknown";

                var result = new Product_DAL().Insert(model);
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
            Product lstmodel = new Product_DAL().SelectById(id);
            if (lstmodel == null)
            {
                return HttpNotFound();
            }
            var listCategory = new ProductCategory_DAL().Select_Category_All();
            ViewBag.ProductCategories = new SelectList(listCategory, "DanhMucId", "TenDanhMuc");

            var listNhaCungCap = new Product_DAL().Select_NhaCungCap_All();
            ViewBag.NhaCungCaps = new SelectList(listNhaCungCap, "NhaCungCapId", "TenNhaCungCap");

            return PartialView("Add", lstmodel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Update(Product model, HttpPostedFileBase ImageFile)
        {
            try
            {
                model.UpdatedBy = User?.Identity?.Name ?? "Unknown";
                model.UpdatedDate = DateTime.Now;

                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    model.HinhAnh = UploadFileToCloud(ImageFile);
                }

                var result = new Product_DAL().Update(model);
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
                var result = new Product_DAL().Delete(Id, TenNguoiXoa);
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
        public ActionResult ImportExcel(HttpPostedFileBase excelFile)
        {
            if (excelFile == null || excelFile.ContentLength == 0)
                return RedirectToAction("Index");
            

            using (var package = new ExcelPackage(excelFile.InputStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                var drawings = worksheet.Drawings;
                var username = User?.Identity?.Name ?? "Unknown";

                for (int row = 2; row <= rowCount; row++)
                {
                    var name = worksheet.Cells[row, 3].Text?.Trim();
                    if (string.IsNullOrEmpty(name)) continue;

                    string finalImageUrl = null;
                    var drawing = drawings.FirstOrDefault(d => d.From.Row + 1 == row);
                    if (drawing is ExcelPicture picture)
                    {
                        finalImageUrl = UploadBytesToCloud(picture.Image.ImageBytes, name + "_excel");
                    }
                    else
                    {
                        var imageUrl = worksheet.Cells[row, 10].Text?.Trim();
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            finalImageUrl = UploadUrlToCloud(imageUrl);
                        }
                    }

                    var product = new Product
                    {
                        DanhMucId = int.TryParse(worksheet.Cells[row, 1].Text, out int cid) ? cid : (int?)null,
                        NhaCungCapId = int.TryParse(worksheet.Cells[row, 2].Text, out int sid) ? sid : (int?)null,
                        TenThuoc = name,
                        HoatChat = worksheet.Cells[row, 4].Text,
                        DonViTinh = worksheet.Cells[row, 5].Text,
                        QuyCach = worksheet.Cells[row, 6].Text,
                        GiaGoc = decimal.TryParse(worksheet.Cells[row, 7].Text, out decimal p) ? p : 0,
                        GiaBan = decimal.TryParse(worksheet.Cells[row, 8].Text, out decimal sp) ? sp : 0,
                        SoLuong = int.TryParse(worksheet.Cells[row, 9].Text, out int q) ? q : 0,
                        HinhAnh = finalImageUrl,
                        KichHoat = worksheet.Cells[row, 11].Text == "1" || worksheet.Cells[row, 11].Text.ToLower() == "true",
                        ThuocKeDon = worksheet.Cells[row, 12].Text == "1" || worksheet.Cells[row, 12].Text.ToLower() == "true"
                    };

                    if (string.IsNullOrEmpty(name))
                        continue;

                    var existing = db.Products.FirstOrDefault(pr => pr.TenThuoc == name);

                    if (existing != null)
                    {
                        product.ThuocId = existing.ThuocId;
                        product.UpdatedDate = DateTime.Now;
                        product.UpdatedBy = username;
                        new Product_DAL().Update(product);
                    }
                    else
                    {
                        product.CreatedDate = DateTime.Now;
                        product.CreatedBy = username;
                        new Product_DAL().Insert(product);
                    }
                }
            }

            TempData["Success"] = "Đã nhập sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult ToggleHienThi(int Id, bool isActive)
        {
            try
            {
                var result = new Product_DAL().Update_IsActive(Id, isActive);
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

        // Dùng cho Form Add/Update
        private string UploadFileToCloud(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0) return null;

            string fileNameOnly = Path.GetFileNameWithoutExtension(file.FileName);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.InputStream),
                Folder = "NhaThuoc/Products",
                PublicId = fileNameOnly,
                Overwrite = true,       
                UniqueFilename = false, 
                UseFilename = true      
            };
            var result = _cloudinary.Upload(uploadParams);
            return result.SecureUrl?.ToString();
        }

        // Dùng cho Import Excel link online 
        private string UploadUrlToCloud(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            if (!url.StartsWith("http")) return url;

            string fileNameOnly = Path.GetFileNameWithoutExtension(new Uri(url).LocalPath);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(url),
                Folder = "NhaThuoc/Products",
                PublicId = fileNameOnly,
                Overwrite = true,
                UniqueFilename = false,
                UseFilename = true
            };
            var result = _cloudinary.Upload(uploadParams);
            return result.SecureUrl?.ToString();
        }

        // Dùng cho Import Excel ảnh nhúng
        private string UploadBytesToCloud(byte[] bytes, string fileName)
        {
            if (bytes == null || bytes.Length == 0) return null;

            string fileNameOnly = Path.GetFileNameWithoutExtension(fileName);

            using (var stream = new MemoryStream(bytes))
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, stream),
                    Folder = "NhaThuoc/Products",
                    PublicId = fileNameOnly,
                    Overwrite = true,
                    UniqueFilename = false,
                    UseFilename = true
                };
                var result = _cloudinary.Upload(uploadParams);
                return result.SecureUrl?.ToString();
            }
        }
    }
}