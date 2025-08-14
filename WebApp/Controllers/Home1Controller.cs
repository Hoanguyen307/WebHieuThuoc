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
            return View(viewModel);
        }
    }
}