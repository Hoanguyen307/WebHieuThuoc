using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class Home1Controller : Controller
    {
        // GET: Home1
        public ActionResult Index()
        {
            var viewModel = new HomeViewModel();

            viewModel.TopSellingProducts = new Product_DAL().Select_TopSelling(8);
            viewModel.LatestProducts = new Product_DAL().Select_GetLatest(8);
            viewModel.DichVuPhongKhamList = new DichVuPhongKham_DAL().Select_All();
            return View(viewModel);
        }

        public ActionResult MainMenu()
        {
            var categoryDAL = new Category_DAL();
            var productCategoryDAL = new ProductCategory_DAL();

            var categories = categoryDAL.Select_Category_All();
            var productCategories = productCategoryDAL.Select_Category_All();

            ViewBag.Categories = categories;
            ViewBag.ProductCategory = productCategories;

            return PartialView("_MainMenuPartial");
        }
    }
}