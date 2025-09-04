using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminPhongKham.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class CategoryController : Controller
    {
        private DBConnect db = new DBConnect();
        public ActionResult Index()
        {
            List<DichVuPhongKham> category = new DichVuPhongKham_DAL().Select_All();
            return View(category);
        }
        public ActionResult GetDanhSachDanhMuc()
        {
            var categories = new DichVuPhongKham_DAL().Select_All();
            return PartialView("Index", categories);
        }

        public ActionResult Add(int? id)
        {
            var category = new DichVuPhongKham();
            if (id != null)
            {
                var danhmuc = db.Categories.Find();
                return PartialView(danhmuc);
            }
            return PartialView("Add", category);
        }

        [HttpPost]
        public JsonResult Add(DichVuPhongKham model/*, HttpPostedFileBase ImageFile*/)
        {
            try
            {
                /*if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads/Category/"), fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    ImageFile.SaveAs(path);
                    model.Image = "/Uploads/Category/" + fileName;
                }*/

                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User?.Identity?.Name ?? "Unknown";
                model.IsDeleted = false;

                var result = new DichVuPhongKham_DAL().Insert(model);
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
            DichVuPhongKham lstmodel = new DichVuPhongKham_DAL().SelectById(id);
            if (lstmodel == null)
            {
                return HttpNotFound();
            }


            return PartialView("Add", lstmodel);
        }
        [HttpPost]
        public JsonResult Update(DichVuPhongKham model/*, HttpPostedFileBase ImageFile*/)
        {
            try
            {
                model.UpdatedBy = User?.Identity?.Name ?? "Unknown";
                model.UpdatedDate = DateTime.Now;
                model.IsDeleted = false;

                /*if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads/Category/"), fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    ImageFile.SaveAs(path);

                    model.Image = "/Uploads/Category/" + fileName;
                }*/

                var result = new DichVuPhongKham_DAL().Update(model);
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
                var result = new DichVuPhongKham_DAL().Delete(Id, TenNguoiXoa);
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