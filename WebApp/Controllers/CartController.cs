using DAL;
using Microsoft.Owin.BuilderProperties;
using Models;
using Models.Payment;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class CartController : Controller
    {
        private DBConnect db = new DBConnect();
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
            List<CartItemModel> itemsToCheckout = null;
            if (TempData["BuyNowItems"] != null)
            {
                itemsToCheckout = TempData["BuyNowItems"] as List<CartItemModel>;
                TempData.Keep("BuyNowItems");
            }
            else
            {
                itemsToCheckout = new Cart_DAL().GetCartForCheckout(CurrentUserId);
            }
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
                return RedirectToAction("Login", "Account");
            List<CartItemModel> cartItems = null;
            bool isBuyNow = false;
            if (TempData["BuyNowItems"] != null)
            {
                cartItems = TempData["BuyNowItems"] as List<CartItemModel>;
                isBuyNow = true;
            }
            else
            {
                cartItems = new Cart_DAL().GetCartForCheckout(CurrentUserId);
                isBuyNow = false;
            }

            if (cartItems == null || !cartItems.Any())
            {
                TempData["Error"] = "Không có sản phẩm để thanh toán hoặc phiên đã hết hạn.";
                if (isBuyNow)
                    return RedirectToAction("Index", "Home1");
                else
                    return RedirectToAction("Index", "Cart");
            }
            try
            {
                var order = new Models.Order
                {
                    OrderCode = "DH" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    CustomerId = CurrentUserId,
                    Status = "Chờ xác nhận",
                    Note = model?.Note ?? string.Empty,
                    CreatedBy = CurrentUserId.ToString(),
                    TotalAmount = cartItems.Sum(c => c.Quantity * c.UnitPrice),
                    DiaChiId = model.SelectedAddressId.Value,
                    PaymentMethod = model.SelectedPaymentMethod ?? "COD"
                };
                order.CreatedDate = DateTime.Now;
                // Chuyển cart -> OrderDetail
                var orderDetails = cartItems.Select(c => new OrderDetail
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.UnitPrice,
                    Discount = 0
                }).ToList();

                // Lưu vào DB
                var orderDal = new Order_DAL();
                int newOrderId = orderDal.Insert(order, orderDetails, model.SelectedVoucherId, model.UsePoints ? model.PointsToUse : 0m);
                if (!isBuyNow)
                {
                    new Cart_DAL().ClearCart(CurrentUserId);
                }
                else
                {
                    TempData.Remove("BuyNowItems");
                }

                if (order.PaymentMethod == "COD")
                {
                    ViewBag.OrderSuccess = true;
                    return View(model);
                }
                else
                {
                    decimal finalTotalAmount = orderDal.GetOrderTotalAmount(newOrderId);
                    string paymentUrl = UrlPayment(order.PaymentMethod, order.OrderCode, model.VnPayType, finalTotalAmount, order.CreatedDate.Value);
                    return Redirect(paymentUrl);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult BuyNow(int productId, int quantity = 1, decimal salePrice = 0)
        {
            if (CurrentUserId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var cartDal = new Cart_DAL();
                //var totalCartCount = cartDal.AddToCart(CurrentUserId, productId, quantity, salePrice);
                var product = new Product_DAL().SelectById(productId);
                var buyNowItem = new CartItemModel
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = salePrice,
                    ProductName = product.TenThuoc,
                    ProductImage = product.HinhAnh
                };

                var buyNowList = new List<CartItemModel>();
                buyNowList.Add(buyNowItem);

                TempData["BuyNowItems"] = buyNowList;

                return RedirectToAction("Checkout");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpGet]
        public JsonResult GetCartCount()
        {
            var count = new Cart_DAL().GetTotalCartCount(CurrentUserId);
            return Json(new { count = count }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; 
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var itemOrder = db.Orders.SingleOrDefault(x => x.OrderCode == orderCode);

                        if (itemOrder != null)
                        {
                            itemOrder.PaymentStatus = "Đã thanh toán";
                            db.SaveChanges();
                            ViewBag.IsSuccess = true;
                            ViewBag.Message = "Giao dịch thành công.";
                        }
                        else
                        {
                            ViewBag.IsSuccess = false;
                            ViewBag.Message = "Không tìm thấy đơn hàng tương ứng.";
                        }

                        if (itemOrder != null)
                        {
                            itemOrder.PaymentStatus = "Đã thanh toán";
                            db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            ViewBag.Message = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ.";
                            ViewBag.Amount = "Số tiền thanh toán: " + vnp_Amount.ToString("N0") + " VND";
                            ViewBag.OrderCode = "Mã đơn hàng: " + orderCode;
                            ViewBag.IsSuccess = true;
                        }
                        else
                        {
                            ViewBag.Message = "Giao dịch thành công nhưng không tìm thấy mã đơn hàng tương ứng.";
                            ViewBag.IsSuccess = false;
                        }
                    }
                    else
                    {
                        ViewBag.Message = $"Có lỗi xảy ra trong quá trình xử lý. Mã lỗi: {vnp_ResponseCode}";
                        ViewBag.IsSuccess = false;
                    }
                }
                else
                {
                    ViewBag.Message = "Chữ ký không hợp lệ. Giao dịch có thể đã bị can thiệp.";
                    ViewBag.IsSuccess = false;
                }
            }
            else
            {
                ViewBag.Message = "Không có thông tin phản hồi từ cổng thanh toán VNPAY.";
                ViewBag.IsSuccess = false;
            }

            return View();
        }

        #region Thanh toán vnpay
        public string UrlPayment(string paymentMethodCode, string orderCode, string vnPayType, decimal amount, DateTime createdDate)
        {
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; 
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; 

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)amount * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); 

            if (paymentMethodCode == "VNPAY")
            {
                switch (vnPayType)
                {
                    case "0": vnpay.AddRequestData("vnp_BankCode", "VNPAYQR"); break;
                    case "1": vnpay.AddRequestData("vnp_BankCode", "VNPAYQR"); break;
                    case "2": vnpay.AddRequestData("vnp_BankCode", "VNBANK"); break;
                    case "3": vnpay.AddRequestData("vnp_BankCode", "INTCARD"); break;
                }
            }
            else if (paymentMethodCode == "CARD")
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }
            vnpay.AddRequestData("vnp_CreateDate", createdDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + orderCode);
            vnpay.AddRequestData("vnp_OrderType", "other"); 

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", orderCode); 

            //Add Params of 2.1.0 Version
            //Billing

            return vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        }
        #endregion
    }
}