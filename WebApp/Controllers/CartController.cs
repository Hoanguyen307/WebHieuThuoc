using DAL;
using Microsoft.Owin.BuilderProperties;
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
        // GET: Cart
        public int CurrentUserId
        {
            get
            {
                var kh = Session["Login"] as Models.KhachHang;
                if (kh == null)
                {
                    return 0; 
                }
                return kh.Id;
            }
        }
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
            if (CurrentUserId == 0)
            {
                return Json(new { items = new List<object>(), total = 0 }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var cartItems = new Cart_DAL().GetCartByCustomer(CurrentUserId);
                var items = cartItems.Select(x => new
                {
                    x.CartItemId,
                    x.ProductId,
                    x.ProductName,
                    ProductImage = "https://localhost:44310" + x.ProductImage,
                    x.UnitPrice,
                    x.Quantity,
                    x.Status,
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
        public JsonResult UpdateStatus(int cartItemId, bool status)
        {
            try
            {
                bool result = new Cart_DAL().UpdateStatus(cartItemId, status);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult AddToCart(int productId, int quantity)
        {
            if (Session["Login"] == null)
            {
                return Json(new { success = false, redirectUrl = Url.Action("Login", "Account"), message = "Bạn phải đăng nhập để thêm sản phẩm vào giỏ hàng." });
            }

            try
            {
                var kh = Session["Login"] as Models.KhachHang;
                int userId = kh.Id;

                var totalCartCount = new Cart_DAL().AddToCart(userId, productId, quantity);

                if (totalCartCount > 0)
                {
                    return Json(new { success = true, totalCartCount = totalCartCount });
                }
                else
                {
                    return Json(new { success = false, message = "Thêm vào giỏ hàng thất bại." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateQuantity(int cartItemId, int quantity)
        {
            if (CurrentUserId == 0)
            {
                return Json(new { success = false, redirectUrl = Url.Action("Login", "Account") });
            }
            try
            {
                var success = new Cart_DAL().UpdateQuantity(cartItemId, quantity);
                var totalCartCount = new Cart_DAL().GetTotalCartCount(CurrentUserId);

                return Json(new { success = success, totalCartCount = totalCartCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult RemoveItem(int cartItemId)
        {
            if (CurrentUserId == 0)
            {
                return Json(new { success = false, redirectUrl = Url.Action("Login", "Account") });
            }
            try
            {
                var success = new Cart_DAL().RemoveItem(cartItemId);
                var totalCartCount = new Cart_DAL().GetTotalCartCount(CurrentUserId);

                return Json(new { success = success, totalCartCount = totalCartCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ClearCart()
        {
            if (CurrentUserId == 0)
            {
                return Json(new { success = false, redirectUrl = Url.Action("Login", "Account") });
            }
            try
            {
                var success = new Cart_DAL().ClearCart(CurrentUserId);
                return Json(new { success = success, totalCartCount = 0 });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult Checkout()
        {
            if (CurrentUserId == 0)
            {
                return RedirectToAction("Login", "Account");
            }
            var model = new CheckoutViewModel();
            model.CartItems = new Cart_DAL().GetCartForCheckout(CurrentUserId);
            if (model.CartItems == null || !model.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            // Lấy địa chỉ giao hàng
            model.Addresses = new DiaChiGiaoHang_DAL().GetByKhachHang(CurrentUserId);
            var defaultAddress = model.Addresses.FirstOrDefault(a => a.MacDinh);

            // Lấy voucher khả dụng cho khách
            model.Vouchers = new Voucher_DAL().GetVouchersByCustomer(CurrentUserId);

            model.CurrentPoints = new KhachHang_DAL().GetCustomerPoints(CurrentUserId);

            model.PaymentMethods = new List<PaymentMethodViewModel>
            {
                new PaymentMethodViewModel { Code = "COD", Name = "Thanh toán khi nhận hàng" },
                new PaymentMethodViewModel { Code = "VNPAY", Name = "Thanh toán qua VNPay" },
                new PaymentMethodViewModel { Code = "MOMO", Name = "Thanh toán qua Momo" },
                new PaymentMethodViewModel { Code = "CARD", Name = "Thẻ tín dụng/ghi nợ" }
            };
            model.SelectedPaymentMethod = "COD";

            return View(model);
        }

        /*
        [HttpPost]
        public ActionResult Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tính toán lại tổng sau khi áp dụng mã giảm giá + điểm
            decimal discount = new Voucher_DAL().(model.VoucherCode, model.TotalAmount);
            decimal pointsDiscount = model.UsePoints ? model.PointsToUse * 1000 : 0; // ví dụ 1 điểm = 1000đ
            decimal finalAmount = model.TotalAmount - discount - pointsDiscount;

            // Lưu đơn hàng
            var orderId = new Order_DAL().CreateOrder(CurrentUserId, model.AddressId, finalAmount, model.PaymentMethod);

            if (model.PaymentMethod == "Online")
            {
                // Redirect sang VNPay/Momo
                string paymentUrl = PaymentService.GenerateUrl(orderId, finalAmount);
                return Redirect(paymentUrl);
            }

            return RedirectToAction("OrderSuccess", new { id = orderId });
        }*/
    }
}