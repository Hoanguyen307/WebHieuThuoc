using DAL;
using Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Models.LichLamViec;
using static Models.ProductReview;

namespace Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductReviewController : Controller
    {
        // GET: ProductReview
        private DBConnect db = new DBConnect();
        public ActionResult Index(string searchString, int? Rating, DateTime? FromDate, DateTime? ToDate)
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetReview(string searchString, int? Rating, DateTime? FromDate, DateTime? ToDate, int page = 1, int pageSize = 10)
        {

            ProductReviewFilter filter = new ProductReviewFilter
            {
                SearchString = searchString,
                Rating = Rating,
                FromDate = FromDate,
                ToDate = ToDate
            };
            try
            {
                var review = new ProductReview_DAL().Select_ProductReview_All(filter);
                var pagedList = review.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);
                return Json(new
                {
                    items = pagedList.ToList(),
                    totalCount = pagedList.TotalItemCount,
                    currentPage = pagedList.PageNumber,
                    pageSize = pagedList.PageSize
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    result = "error",
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteAccount(int Id, string TenNguoiXoa)
        {
            try
            {
                var result = new ProductReview_DAL().DeleteReview(Id, TenNguoiXoa);
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
    }
}