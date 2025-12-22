using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class VouchersController : Controller
    {
        // GET: Vouchers
        public ActionResult Index()
        {
            return View();
        }
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
        [HttpPost]
        public ActionResult SaveVoucher(int voucherId)
        {
            if (Session["Login"] == null)
            {
                return Json(new { success = false, redirectUrl = Url.Action("Login", "Account"), message = "Bạn phải đăng nhập để thêm sản phẩm vào giỏ hàng." });
            }
            try
            {
                var kh = Session["Login"] as Models.KhachHang;
                int customerId = kh.Id;

                var voucherDAL = new Voucher_DAL();
                voucherDAL.SaveVoucherForCustomer(customerId, voucherId);

                return Json(new { success = true, message = "Đã lưu thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}