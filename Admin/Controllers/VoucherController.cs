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
    [Authorize(Roles = "Admin, Employee")]
    public class VoucherController : Controller
    {
        private DBConnect db = new DBConnect();
        // GET: Voucher
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetDanhSachVoucher(int page = 1, int pageSize = 10)
        {
            var vouchers = new Voucher_DAL().Select_Voucher_All();
            var pagedList = vouchers.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);
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
            var vouchers = new Voucher();

            if (id != null)
            {
                var voucher = db.Vouchers.Find(id);
                return PartialView("Add", voucher);
            }
            return PartialView("Add", vouchers);
        }

        [HttpPost]
        public JsonResult Add(Voucher model)
        {
            try
            {

                model.CreatedDate = DateTime.Now;
                model.IsDeleted = false;

                var result = new Voucher_DAL().Insert(model);
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
            Voucher lstmodel = new Voucher_DAL().SelectById(id);
            if (lstmodel == null)
            {
                return HttpNotFound();
            }

            return PartialView("Add", lstmodel);
        }
        [HttpPost]
        public JsonResult Update(Voucher model)
        {
            try
            {
                model.UpdatedDate = DateTime.Now;
                model.IsDeleted = false;

                var result = new Voucher_DAL().Update(model);
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
                var result = new Voucher_DAL().Delete(Id, TenNguoiXoa);
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
        public JsonResult ToggleHienThi(int Id, bool isActive)
        {
            try
            {
                var result = new Voucher_DAL().Voucher_IsActive(Id, isActive);
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