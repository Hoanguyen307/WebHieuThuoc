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
    public class FlashSaleProductController : Controller
    {
        private DBConnect db = new DBConnect();

        // GET: FlashSaleProduct
        public ActionResult Index(int flashSaleId)
        {
            ViewBag.FlashSaleId = flashSaleId;
            return View();
        }

        // Lấy danh sách sản phẩm của FlashSale
        public ActionResult GetProducts(int flashSaleId, string keyword = "", int page = 1, int pageSize = 10)
        {
            var products = new FlashSaleProduct_DAL().Select_ByFlashSale(flashSaleId, keyword);
            var pagedList = products.OrderBy(x => x.TenThuoc).ToPagedList(page, pageSize);

            return Json(new
            {
                items = pagedList.ToList(),
                totalCount = pagedList.TotalItemCount,
                currentPage = pagedList.PageNumber,
                pageSize = pagedList.PageSize
            }, JsonRequestBehavior.AllowGet);
        }

        // Thêm sản phẩm vào FlashSale
        [HttpPost]
        public JsonResult Add(int flashSaleId, List<FlashSaleProduct> products)
        {
            try
            {
                int count = 0;
                if (products != null && products.Count > 0)
                {
                    foreach (var item in products)
                    {
                        item.FlashSaleId = flashSaleId;
                        item.SoldQuantity = 0;

                        var result = new FlashSaleProduct_DAL().Insert(item);
                        if (result > 0) count++;
                    }
                }

                if (count > 0)
                    return Json(new { success = true, msg = $"Đã thêm {count} sản phẩm vào FlashSale" });
                else
                    return Json(new { success = false, msg = "Không thêm được sản phẩm nào" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Lỗi: " + ex.Message });
            }
        }


        // Xóa sản phẩm khỏi FlashSale
        [HttpPost]
        public JsonResult Delete(int flashSaleId, int productId)
        {
            try
            {
                var result = new FlashSaleProduct_DAL().Delete(flashSaleId, productId);
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