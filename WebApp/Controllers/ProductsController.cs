using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Models.Product;

namespace WebApp.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        public ActionResult Index(int? ProductCategoryId, int? NhaCungCapId, string sortOrder = "newest", int pageSize = 40)
        {
            var filter = new ProductFilter
            {
                ProductCategoryId = ProductCategoryId,
                NhaCungCapId = NhaCungCapId
            };

            var product = new Product_DAL().Select_Published(filter, sortOrder).Take(pageSize).ToList();

            var category = new Category_DAL().Select_Category_All();
            ViewBag.Categories = category;

            var listNhaCungCap = new Product_DAL().Select_NhaCungCap_All();
            ViewBag.NhaCungCaps = listNhaCungCap; 

            var listproductCategory = new ProductCategory_DAL().Select_Category_All();
            ViewBag.ProductCategory = listproductCategory; 

            ViewBag.SelectedBrandId = NhaCungCapId;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SelectedCategoryId = ProductCategoryId;
            return View(product);
        }
        public ActionResult Details(int? id, int? rating = null)
        {
            var product = new Product_DAL().SelectById(id.Value); 
            if (product == null)
            {
                return HttpNotFound();
            }
            var relatedProducts = new Product_DAL().Select_Published(new ProductFilter { ProductCategoryId = product.DanhMucId }, null)
                                     .Where(p => p.ThuocId != product.ThuocId)
                                     .Take(4)
                                     .ToList();
            var reviews = new ProductReview_DAL().ReviewGetByProduct(id.Value, rating);
            var averageRating = new ProductReview_DAL().GetAverageRating(id.Value);
            var totalReviews = reviews.Count;
            if (rating != null)
            {
                totalReviews = new ProductReview_DAL().ReviewGetByProduct(id.Value, null).Count;
            }
            var viewModel = new ProductViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts,
                productReviews = reviews,
                AverageRating = averageRating,
                TotalReviews = totalReviews
            };

            ViewBag.SelectedRating = rating;
            return View(viewModel);
        }
        [HttpGet]
        public ActionResult Search(string searchString)
        {
            var productDAL = new Product_DAL();
            var products = new List<Product>();

            products = productDAL.Search(searchString);
            var category = new Category_DAL().Select_Category_All();
            ViewBag.Categories = category;

            var listNhaCungCap = new Product_DAL().Select_NhaCungCap_All();
            ViewBag.NhaCungCaps = listNhaCungCap;

            var listproductCategory = new ProductCategory_DAL().Select_Category_All();
            ViewBag.ProductCategory = listproductCategory;

            ViewBag.SearchString = searchString;
            return View("Index", products);
        }

        [HttpGet]
        public ActionResult _GetProductReviews(int productId, int? rating = null)
        {

            var reviews = new ProductReview_DAL().ReviewGetByProduct(productId, rating);

            return PartialView("_ProductReviewsList", reviews);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(int ProductId, int Rating, string Comment)
        {
            if (Session["Login"] == null)
            {
                return Json(new
                {
                    success = false,
                    redirectUrl = Url.Action("Login", "Account"),
                    message = "Bạn phải đăng nhập để gửi đánh giá."
                });
            }

            try
            {
                var kh = Session["Login"] as Models.KhachHang;

                if (kh == null)
                {
                    return Json(new
                    {
                        success = false,
                        redirectUrl = Url.Action("Login", "Account"),
                        message = "Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại."
                    });
                }

                int userId = kh.Id;
                var review = new ProductReview
                {
                    ProductId = ProductId,
                    CustomerId = userId,
                    Rating = Rating,
                    Comment = Comment,
                    CreatedDate = DateTime.Now
                };

                var rv = new ProductReview_DAL().Insert(review);

                if (rv > 0)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Cảm ơn bạn đã gửi đánh giá!"
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Gửi đánh giá thất bại." });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại."
                });
            }
        }
        /*protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                
            }
            base.Dispose(disposing);
        }*/
    }
}