using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Models.Product;

namespace WebApp.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        public ActionResult Index(int? CategoryId, int? BrandId, string sortOrder = "newest")
        {
            var filter = new ProductFilter
            {
                CategoryId = CategoryId,
                BrandId = BrandId
            };

            var product = new Product_DAL().Select_Published(filter, sortOrder);
            var category = new Category_DAL().Select_Category_All();
            ViewBag.Categories = category;
            var listBrands = new Product_DAL().Select_Brands_All();
            ViewBag.Brands = listBrands; 
            ViewBag.SelectedBrandId = BrandId;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SelectedCategoryId = CategoryId;
            return View(product);
        }
        public ActionResult Details(int id)
        {
            var product = new Product_DAL().SelectById(id); 
            if (product == null)
            {
                return HttpNotFound();
            }
            var relatedProducts = new Product_DAL().Select_Published(new ProductFilter { CategoryId = product.CategoryId }, null)
                                     .Where(p => p.Id != product.Id)
                                     .Take(4)
                                     .ToList();
            var viewModel = new ProductViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts
            };

            return View(viewModel);
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