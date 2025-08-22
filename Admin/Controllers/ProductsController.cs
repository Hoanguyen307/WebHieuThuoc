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

        public ActionResult Index(string searchString, decimal? MinPrice, decimal? MaxPrice, int? ProductCategoryId, int? Month, int? Year)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var year = Enumerable.Range(currentYear - 5, 11).Select(y => new { Id = y, Name = y.ToString() }).ToList();
            ViewBag.Years = new SelectList(year, "Id", "Name", Year);

            var months = Enumerable.Range(1, 12).Select(m => new { Id = m, Name = $"Tháng {m}" }).ToList();
            ViewBag.Months = new SelectList(months, "Id", "Name", Month);

            var listCategory = new ProductCategory_DAL().Select_Category_All();
            ViewBag.ProductCategories = new SelectList(listCategory, "Id", "Name", ProductCategoryId);


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
                Year = Year,
                MinPrice = MinPrice,
                MaxPrice = MaxPrice
            };

            var processes = new Product_DAL().Select_Product_All(filter);
            int threshold = 10;

            var pagedList = processes.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);
            var result = pagedList.Select(p => new
            {
                p.Id,
                p.Image,
                p.Name,
                p.ProductCategoryName,
                p.Price,
                p.Quantity,
                p.Sold,
                p.SalePrice,
                p.IsActive,
                p.IsFeatured,
                CanhBaoHetHang = p.Quantity <= threshold
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
            ViewBag.Categories = new SelectList(listCategory, "Id", "Name");

            var listBrands = new Product_DAL().Select_Brands_All();
            ViewBag.Brands = new SelectList(listBrands, "Id", "TenThuongHieu");

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
                    model.Image = "/Uploads/Product/" + fileName;
                }

                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User?.Identity?.Name ?? "Unknown";
                model.IsDeleted = false;

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
            ViewBag.Categories = new SelectList(listCategory, "Id", "Name");

            var listBrands = new Product_DAL().Select_Brands_All();
            ViewBag.Brands = new SelectList(listBrands, "Id", "TenThuongHieu");

            return PartialView("Add", lstmodel);
        }
        [HttpPost]
        public JsonResult Update(Product model, HttpPostedFileBase ImageFile)
        {
            try
            {
                model.UpdatedBy = User?.Identity?.Name ?? "Unknown";
                model.UpdatedDate = DateTime.Now;
                model.IsDeleted = false;

                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads/Product/"), fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    ImageFile.SaveAs(path);

                    model.Image = "/Uploads/Product/" + fileName;
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
        /*[HttpPost]
        public ActionResult IsHome(int ID)
        {
            var item = db.Products.Find(ID);
            if (item != null)
            {
                item.IsHome = !item.IsHome;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isHome = item.IsHome });
            }
            return Json(new { success = false });
        }*/
        //[HttpPost]
        //public ActionResult IsActive(int ID)
        //{
        //    var item = db.Products.Find(ID);
        //    if (item != null)
        //    {
        //        item.IsActive = !item.IsActive;
        //        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //        return Json(new { success = true, isActive = item.IsActive });
        //    }
        //    return Json(new { success = false });
        //}
        /*[HttpPost]
        public ActionResult IsSale(int ID)
        {
            var item = db.Products.Find(ID);
            if (item != null)
            {
                item.IsSale = !item.IsSale;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isSale = item.IsSale });
            }
            return Json(new { success = false });
        }*/
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
                    var ProductCategoryName = worksheet.Cells[row, 1].Text?.Trim();
                    var Name = worksheet.Cells[row, 2].Text?.Trim();
                    var Slug = worksheet.Cells[row, 3].Text?.Trim();
                    var Description = worksheet.Cells[row, 4].Text?.Trim();
                    var Price = worksheet.Cells[row, 5].Text?.Trim();
                    var SalePrice = worksheet.Cells[row, 6].Text?.Trim();
                    var Image = worksheet.Cells[row, 7].Text?.Trim();
                    var Quantity = worksheet.Cells[row, 8].Text?.Trim();
                    var IsActive = worksheet.Cells[row, 9].Text?.Trim();
                    var IsFeatured = worksheet.Cells[row, 10].Text?.Trim();
                    var Tags = worksheet.Cells[row, 11].Text?.Trim();
                    if (string.IsNullOrEmpty(Name))
                        continue;
                    var category = new ProductCategory_DAL().Select_Category_All()
               .FirstOrDefault(c => c.Name.Equals(ProductCategoryName, StringComparison.OrdinalIgnoreCase));
                    var existing = db.Products.FirstOrDefault(p => p.Name == Name);
                    var username = User?.Identity?.Name ?? "Unknown";
                    string savedImageName = null;
                    if (!string.IsNullOrEmpty(Image))
                    {
                        if (Uri.IsWellFormedUriString(Image, UriKind.Absolute))
                        {
                            var fileName = Path.GetFileName(new Uri(Image).LocalPath);
                            savedImageName = $"{Guid.NewGuid().ToString().Substring(0, 8)}_{fileName}";

                            var savePath = Server.MapPath("~/Uploads/Product/");
                            if (!Directory.Exists(savePath))
                                Directory.CreateDirectory(savePath);

                            var fullPath = Path.Combine(savePath, savedImageName);

                            using (WebClient client = new WebClient())
                            {
                                client.DownloadFile(Image, fullPath);

                            }
                            savedImageName = $"/Uploads/Product/{savedImageName}";
                        }
                        else
                        {
                            savedImageName = Image;
                        }
                    }

                    var product = new Product
                    {
                        ProductCategoryId = category?.Id ?? 0,
                        Name = Name,
                        Slug = Slug,
                        Description = Description,
                        Price = decimal.TryParse(Price, out var price) ? price : 0,
                        SalePrice = string.IsNullOrEmpty(SalePrice) ? (decimal?)null : decimal.Parse(SalePrice),
                        Image = savedImageName,
                        Quantity = int.TryParse(Quantity, out var quantity) ? quantity : 0,
                        IsActive = bool.TryParse(IsActive, out var isActive) && isActive,
                        IsFeatured = bool.TryParse(IsFeatured, out var isFeatured) && isFeatured,
                        Tags = Tags
                    };
                    if (existing != null)
                    {
                        product.Id = existing.Id;
                        product.UpdatedDate = DateTime.Now;
                        product.UpdatedBy = username;
                        new Product_DAL().Update(product);
                    }
                    else
                    {
                        // Thêm mới
                        product.CreatedDate = DateTime.Now;
                        product.CreatedBy = username;
                        product.IsDeleted = false;
                        new Product_DAL().Insert(product);
                    }

                }
            }

            TempData["Success"] = "Đã nhập nhân viên thành công!";
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
                    product.ProductId = sanpham.Id;
                    product.ProductName = sanpham.Name; 
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