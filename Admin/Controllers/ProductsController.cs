using DAL;
using Models;
using OfficeOpenXml;
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

namespace Admin.Controllers
{
    /*[Authorize(Roles = "Admin, Employee")]*/
    public class ProductsController : Controller
    {
        private DBConnect db = new DBConnect();

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
                p.GiaBan,
                p.SoLuong,
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
        public JsonResult Add(Product model, HttpPostedFileBase ImageFile)
        {
            try
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads/Product/"), fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    ImageFile.SaveAs(path);
                    model.HinhAnh = "/Uploads/Product/" + fileName;
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
        public JsonResult Update(Product model, HttpPostedFileBase ImageFile)
        {
            try
            {
                model.UpdatedBy = User?.Identity?.Name ?? "Unknown";
                model.UpdatedDate = DateTime.Now;

                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads/Product/"), fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    ImageFile.SaveAs(path);

                    model.HinhAnh = "/Uploads/Product/" + fileName;
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

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var imageFolder = Server.MapPath("~/Uploads/Product/");
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }

            using (var package = new ExcelPackage(excelFile.InputStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var categoryIdStr = worksheet.Cells[row, 1].Text?.Trim(); 
                    var supplierIdStr = worksheet.Cells[row, 2].Text?.Trim(); 
                    var name = worksheet.Cells[row, 3].Text?.Trim();
                    var hoatChat = worksheet.Cells[row, 4].Text?.Trim();
                    var donViTinh = worksheet.Cells[row, 5].Text?.Trim();
                    var quyCach = worksheet.Cells[row, 6].Text?.Trim();
                    var priceStr = worksheet.Cells[row, 7].Text?.Trim();
                    var salepriceStr = worksheet.Cells[row, 8].Text?.Trim();
                    var qtyStr = worksheet.Cells[row, 9].Text?.Trim();
                    var image = worksheet.Cells[row, 10].Text?.Trim();
                    var isActiveStr = worksheet.Cells[row, 11].Text?.Trim();

                    if (string.IsNullOrEmpty(name))
                        continue;

                    var existing = db.Products.FirstOrDefault(pr => pr.TenThuoc == name);
                    var username = User?.Identity?.Name ?? "Unknown";

                    // Xử lý hình ảnh
                    string savedImageName = null;
                    if (!string.IsNullOrEmpty(image))
                    {
                        if (Uri.IsWellFormedUriString(image, UriKind.Absolute))
                        {
                            var fileName = Path.GetFileName(new Uri(image).LocalPath);
                            savedImageName = $"{Guid.NewGuid().ToString().Substring(0, 8)}_{fileName}";
                            var savePath = Path.Combine(imageFolder, savedImageName);

                            using (WebClient client = new WebClient())
                            {
                                client.DownloadFile(image, savePath);
                            }
                            savedImageName = $"/Uploads/Product/{savedImageName}";
                        }
                        else
                        {
                            savedImageName = image;
                        }
                    }

                    // Parse dữ liệu
                    int? categoryId = int.TryParse(categoryIdStr, out var cid) ? cid : (int?)null;
                    int? supplierId = int.TryParse(supplierIdStr, out var sid) ? sid : (int?)null;
                    decimal price = decimal.TryParse(priceStr, out var p) ? p : 0;
                    decimal saleprice = decimal.TryParse(priceStr, out var sp) ? sp : 0;
                    int quantity = int.TryParse(qtyStr, out var q) ? q : 0;
                    bool isActive = isActiveStr == "1" || isActiveStr.Equals("true", StringComparison.OrdinalIgnoreCase);

                    var product = new Product
                    {
                        DanhMucId = categoryId,
                        NhaCungCapId = supplierId,
                        TenThuoc = name,
                        HoatChat = hoatChat,
                        DonViTinh = donViTinh,
                        QuyCach = quyCach,
                        GiaGoc = price,
                        GiaBan = saleprice,
                        SoLuong = quantity,
                        HinhAnh = savedImageName,
                        KichHoat = isActive
                    };

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
        public ActionResult DungTich(int? id)
        {
            var product = new DungTichSanPham();
            var listDungTich = new DungTich_DAL().Select_DungTich_All();
            ViewBag.DungTiches = new SelectList(listDungTich, "Id", "Value");

            if (id != null)
            {
                var sanpham = db.Products.Find(id);
                if (sanpham != null)
                {
                    product.ProductId = sanpham.ThuocId;
                    product.ProductName = sanpham.TenThuoc; 
                }
            }
            return PartialView("DungTich", product);
        }
        [HttpPost]
        public JsonResult AddDungTich(DungTichSanPham model)
        {
            try
            {

                model.CreatedDate = DateTime.Now;
                model.IsDeleted = false;

                var result = new DungTich_DAL().Insert(model);
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

    }
}