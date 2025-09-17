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
        public ActionResult Details(int? id)
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
            var reviews = new ProductReview_DAL().ReviewGetByProduct(id.Value);
            var viewModel = new ProductViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts,
                productReviews = reviews
            };

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
        /*protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                
            }
            base.Dispose(disposing);
        }*/
    }
}