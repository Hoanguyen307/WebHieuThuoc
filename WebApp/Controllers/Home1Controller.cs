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
            viewModel.LatestProducts = new Product_DAL().Select_GetLatest(10);

            var allFlashSales = new FlashSale_DAL().Select_ActiveFlashSale();

            if (allFlashSales != null && allFlashSales.Any())
            {
                ViewBag.ActiveFlashSaleId = allFlashSales.First().Id;
                ViewBag.AllFlashSales = allFlashSales;
            }
            else
            {
                ViewBag.ActiveFlashSaleId = null;
                ViewBag.AllFlashSales = new List<FlashSale>();
            }

            return View(viewModel);
        }

        public ActionResult MainMenu()
        {
            var categoryDAL = new Category_DAL();
            var menu = categoryDAL.Select_Menu_All();
            ViewBag.Menus = menu;

            return PartialView("_MainMenuPartial");
        }
        [HttpGet]
        public ActionResult GetCategoriesByMenuId(int menuId)
        {
            var categoryDAL = new Category_DAL();
            var productCategoryDAL = new ProductCategory_DAL();

            var categories = categoryDAL.Select_ByMenuId(menuId);
            var productCategories = productCategoryDAL.Select_Category_All();

            ViewBag.Categories = categories;
            ViewBag.ProductCategories = productCategories;

            return PartialView("_CategoriesByMenuIdPartial");
        }
        public ActionResult FlashSalePartial()
        {
            var allFlashSales = new FlashSale_DAL().Select_ActiveFlashSale();
            ViewBag.AllFlashSales = allFlashSales ?? new List<FlashSale>();
            return PartialView("_FlashSalePartial");

        }

        public ActionResult FlashSaleProducts(int flashSaleId)
        {
            var flashSale = new FlashSale_DAL().SelectById(flashSaleId);
            var products = new Product_DAL().Select_Product_FlashSale(flashSaleId);

            if (flashSale.StartTime > DateTime.Now)
            {
                products = new Product_DAL().Select_Product_FlashSale(flashSaleId); 
                ViewBag.SaleComing = true;
            }
            var list = products.Select(p => new ProductFlashSaleViewModel
            {
                ThuocId = p.ThuocId,
                ProductId = p.ProductId,
                TenThuoc = p.TenThuoc,
                GiaBan = p.GiaBan,
                DiscountPercent = p.DiscountPercent,
                HinhAnh = p.HinhAnh,
                DonViTinh = p.DonViTinh,
                SalePrice = p.SalePrice,
                FlashStock = p.FlashStock ?? 0,
                SoldQuantity = p.SoldQuantity ?? 0
            }).ToList();

            return PartialView("_FlashSaleProducts", list);
        }
        public ActionResult AvailableVouchersPartial()
        {
            try
            {
                int customerId = 0;
                if (Session["Login"] != null)
                {
                    var kh = Session["Login"] as Models.KhachHang;
                    if (kh != null)
                    {
                        customerId = kh.Id;
                    }
                }
                var voucherDAL = new Voucher_DAL();
                var availableVouchers = voucherDAL.Select_ActiveVouchersForCustomer(customerId);
                return PartialView("_AvailableVouchersPartial", availableVouchers);
            }
            catch (Exception ex)
            {
                return PartialView("_AvailableVouchersPartial", new List<VoucherViewModel>());
            }
        }

    }
}