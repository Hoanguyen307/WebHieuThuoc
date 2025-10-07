using DAL;
using Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    /*[Authorize(Roles = "Admin, Employee")]*/
    public class FlashSaleController : Controller
    {
        private DBConnect db = new DBConnect();
        // GET: FlashSale
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetFlashSale(int page = 1, int pageSize = 10)
        {
            var fl = new FlashSale_DAL().Select_All();
            var pagedList = fl.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);
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
            var fl = new FlashSale();

            if (id != null)
            {
                var fls = db.flashSales.Find(id);
                return PartialView("Add", fls);
            }
            return PartialView("Add", fl);
        }

        [HttpPost]
        public JsonResult Add(FlashSale model)
        {
            try
            {

                model.CreatedDate = DateTime.Now;

                var result = new FlashSale_DAL().Insert(model);
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
            FlashSale lstmodel = new FlashSale_DAL().SelectById(id);
            if (lstmodel == null)
            {
                return HttpNotFound();
            }

            return PartialView("Add", lstmodel);
        }
        [HttpPost]
        public JsonResult Update(FlashSale model)
        {
            try
            {

                var result = new FlashSale_DAL().Update(model);
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
                var result = new FlashSale_DAL().Delete(Id, TenNguoiXoa);
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
                var result = new FlashSale_DAL().Update_IsActive(Id, isActive);
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
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}