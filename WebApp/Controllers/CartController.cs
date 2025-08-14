using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class CartController : Controller
    {
        private int CurrentUserId => 1;
        // GET: Cart
        public ActionResult Index()
        {
            try
            {
                var cartItems = new Cart_DAL().GetCartByCustomer(CurrentUserId);
                ViewBag.CartCount = cartItems.Sum(x => x.Quantity);
                return View(cartItems);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.CartCount = 0;
                return View(new List<CartItemModel>());
            }
        }

        [HttpGet]
        public JsonResult GetMiniCart()
        {
            try
            {
                var cartItems = new Cart_DAL().GetCartByCustomer(CurrentUserId);
                var items = cartItems.Select(x => new
                {
                    x.Id,
                    x.ProductId,
                    x.ProductName,
                    x.Image,
                    x.UnitPrice,
                    x.Quantity
                }).ToList();

                var total = items.Sum(i => i.UnitPrice * i.Quantity);

                return Json(new { items, total }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { items = new List<object>(), total = 0, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddToCart(int productId, int quantity)
        {
            try
            {
                int userId = 1; 

                var result = new Cart_DAL().AddToCart(userId, productId, quantity);

                return Json(new { success = result > 0 });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateQuantity(int cartItemId, int quantity)
        {
            try
            {
                var success = new Cart_DAL().UpdateQuantity(cartItemId, quantity);
                return Json(new { success });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult RemoveItem(int cartItemId)
        {
            try
            {
                var success = new Cart_DAL().RemoveItem(cartItemId);
                return Json(new { success });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ClearCart()
        {
            try
            {
                var success = new Cart_DAL().ClearCart(CurrentUserId);
                return Json(new { success });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}