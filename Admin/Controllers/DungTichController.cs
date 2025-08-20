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
    public class DungTichController : Controller
    {
        private DBConnect db = new DBConnect();
        // GET: DungTich
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetDanhSachDungTich(int page = 1, int pageSize = 10)
        {
            var dungtichs = new DungTich_DAL().Select_DungTichSP_All();
            var pagedList = dungtichs.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);
            return Json(new
            {
                items = pagedList.ToList(),
                totalCount = pagedList.TotalItemCount,
                currentPage = pagedList.PageNumber,
                pageSize = pagedList.PageSize
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return HttpNotFound();
            }
            DungTichSanPham lstmodel = new DungTich_DAL().SelectById(id);
            if (lstmodel == null)
            {
                return HttpNotFound();
            }

            var listDungTich = new DungTich_DAL().Select_DungTich_All();
            ViewBag.DungTiches = new SelectList(listDungTich, "Id", "Value");
            return PartialView("Add", lstmodel);
        }
        [HttpPost]
        public JsonResult Update(DungTichSanPham model/*, HttpPostedFileBase ImageFile*/)
        {
            try
            {
                model.UpdatedDate = DateTime.Now;
                model.IsDeleted = false;

                var result = new DungTich_DAL().Update(model);
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
                var result = new DungTich_DAL().Delete(Id, TenNguoiXoa);
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