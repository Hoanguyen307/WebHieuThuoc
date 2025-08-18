using DAL;
using Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class ProductCategoryController : Controller
    {
        private DBConnect db = new DBConnect();
        // GET: ProductCategory
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetDanhSachDanhMuc(int page = 1, int pageSize = 10)
        {
            var categories = new ProductCategory_DAL().Select_Category_All();
            var pagedList = categories.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);
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
            var category = new ProductCategory();

            var listDanhMuc = new Category_DAL().Select_Category_All();
            ViewBag.ProductCategories = new SelectList(listDanhMuc, "Id", "Name");

            if (id != null)
            {
                var danhmuc = db.Categories.Find(id);
                return PartialView("Add", danhmuc);
            }
            return PartialView("Add", category);
        }

        [HttpPost]
        public JsonResult Add(ProductCategory model)
        {
            try
            {

                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User?.Identity?.Name ?? "Unknown";
                model.IsDeleted = false;

                var result = new ProductCategory_DAL().Insert(model);
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
            ProductCategory lstmodel = new ProductCategory_DAL().SelectById(id);
            if (lstmodel == null)
            {
                return HttpNotFound();
            }
            
            var listDanhMuc = new Category_DAL().Select_Category_All();
            ViewBag.ProductCategories = new SelectList(listDanhMuc, "Id", "Name");
            return PartialView("Add", lstmodel);
        }
        [HttpPost]
        public JsonResult Update(ProductCategory model/*, HttpPostedFileBase ImageFile*/)
        {
            try
            {
                model.UpdatedBy = User?.Identity?.Name ?? "Unknown";
                model.UpdatedDate = DateTime.Now;
                model.IsDeleted = false;

                var result = new ProductCategory_DAL().Update(model);
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
                var result = new ProductCategory_DAL().Delete(Id, TenNguoiXoa);
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
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}