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
        public ActionResult Checkout(int? diaChiId)
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
            if (diaChiId.HasValue)
            {
                new DiaChiGiaoHang_DAL().SetDefault(CurrentUserId, diaChiId.Value);
            }
            var defaultAddress = new DiaChiGiaoHang_DAL().GetDefault(CurrentUserId);
            //var defaultAddress = model.Addresses.FirstOrDefault(a => a.MacDinh);

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

            model.SubTotal = model.CartItems.Sum(x => x.UnitPrice * x.Quantity);
            model.Discount = 0; // nếu có giảm giá thì thay đổi
            model.FinalTotal = model.SubTotal - model.Discount;
            return View(model);
        }

        [HttpPost]
        public ActionResult Checkout(CheckoutViewModel model)
        {
            if (CurrentUserId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Lấy lại giỏ hàng từ DB (an toàn)
                var cartItems = new Cart_DAL().GetCartForCheckout(CurrentUserId);
                if (cartItems == null || !cartItems.Any())
                {
                    return RedirectToAction("Index", "Cart");
                }

                // Tạo Order object
                var order = new Models.Order
                {
                    OrderCode = "DH" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    CustomerId = CurrentUserId,
                    Status = 0,
                    Note = model?.Note ?? string.Empty,
                    CreatedBy = CurrentUserId.ToString(),
                    TotalAmount = 0 // store sẽ tính lại
                };

                // Chuyển cart -> danh sách OrderDetail để truyền TVP
                var orderDetails = new List<OrderDetail>();
                foreach (var c in cartItems)
                {
                    orderDetails.Add(new OrderDetail
                    {
                        ProductId = c.ProductId,
                        Quantity = c.Quantity,
                        UnitPrice = c.UnitPrice,
                        Discount = 0 // nếu có discount theo sản phẩm thì fill vào
                    });
                }

                // Gọi DAL.Insert (overload) — store sẽ chèn chi tiết + xử lý voucher & points
                var orderDal = new Order_DAL();
                int newOrderId = orderDal.Insert(
                    order,
                    orderDetails,
                    model.SelectedVoucherId,
                    model.UsePoints ? model.PointsToUse : 0m
                );

                // Xóa giỏ hàng
                new Cart_DAL().ClearCart(CurrentUserId);

                // Nếu COD -> success page
                if (model.SelectedPaymentMethod == "COD")
                {
                    return RedirectToAction("OrderSuccess", new { id = newOrderId });
                }

                // Nếu thanh toán online -> ở đây xử lý redirect tới cổng (VNPay/Momo) trước khi confirm
                // TODO: generate payment url, redirect
                return RedirectToAction("OrderSuccess", new { id = newOrderId });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }

        public ActionResult OrderSuccess(int id)
        {
            try
            {
                var order = new Order_DAL().GetOrderDetails(id);
                if (order == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }



        [HttpGet]
        public JsonResult GetCartCount()
        {
            var count = new Cart_DAL().GetTotalCartCount(CurrentUserId);
            return Json(new { count = count }, JsonRequestBehavior.AllowGet);
        }


    }
}