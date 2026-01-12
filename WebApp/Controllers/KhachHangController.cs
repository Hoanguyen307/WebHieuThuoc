using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class KhachHangController : Controller
    {
        // GET: KhachHang
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UpdateInfo()
        {
            var kh = Session["Login"] as KhachHang;
            if (kh == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateInfo(KhachHang model)
        {
            try
            {
                var khSession = Session["Login"] as KhachHang;
                if (khSession == null)
                    return Json(new { code = 401, msg = "Phiên đăng nhập hết hạn!" });

                model.Id = khSession.Id;
                model.UpdatedBy = "Khách hàng";

                var dal = new KhachHang_DAL();
                var rows = dal.UpdateInfo(model);

                if (rows > 0)
                {
                    khSession.FullName = model.FullName;
                    khSession.Gender = model.Gender;
                    khSession.BirthDate = model.BirthDate;
                    khSession.Phone = model.Phone;
                    khSession.Email = model.Email;
                    khSession.Address = model.Address;
                    Session["Login"] = khSession;

                    return Json(new { code = 200, msg = "Cập nhật thông tin thành công!" });
                }
                else
                {
                    return Json(new { code = 400, msg = "Không thể cập nhật thông tin." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi hệ thống: " + ex.Message });
            }
        }

        public ActionResult Diem(int page = 1, int pageSize = 10)
        {
            var kh = Session["Login"] as KhachHang;
            if (kh == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var dal = new KhachHang_DAL();

            var allLichSu = dal.LichSu_Diem(kh.Id);
            int totalRecords = allLichSu.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var pagedLichSu = allLichSu
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            ViewBag.LichSuDiem = pagedLichSu;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            return View();
        }

        public ActionResult DonHang(string status, int page = 1, int pageSize = 10)
        {
            var khachHang = Session["Login"] as KhachHang;
            if (khachHang == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orderDAL = new Order_DAL();
            var allOrders = orderDAL.LichSu_DonHang(khachHang.Id);
            if (!string.IsNullOrEmpty(status))
            {
                allOrders = allOrders.Where(o => o.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            int totalRecords = allOrders.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var pagedOrders = allOrders
                                .OrderByDescending(o => o.CreatedDate)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentStatus = status;

            return View(pagedOrders);
        }
        [HttpGet]
        public JsonResult Track(string tracking)
        {
            try
            {
                var order = new Shipping_DAL().GetShippingOrderByTracking(tracking);
                if (order == null)
                    return Json(new { code = 404, msg = "Không tìm thấy mã vận đơn" }, JsonRequestBehavior.AllowGet);

                var history = new Shipping_DAL().GetShippingHistory(order.Id);

                return Json(new
                {
                    code = 200,
                    data = new
                    {
                        order = order,
                        history = history
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult ChiTietDonHang(int id)
        {
            var khachHang = Session["Login"] as KhachHang;
            if (khachHang == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized, "Vui lòng đăng nhập lại.");
            }

            var orderDAL = new Order_DAL();
            var shippingDAL = new Shipping_DAL();

            try
            {
                var orderDetails = orderDAL.GetOrderDetails_ByCustomer(id, khachHang.Id);

                if (orderDetails == null)
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.NotFound,
                        "Không tìm thấy chi tiết đơn hàng hoặc bạn không có quyền xem.");
                }

                var shipping = shippingDAL.GetByOrderId(id);

                ViewBag.TrackingCode = shipping?.TrackingCode ?? "";
                ViewBag.ShippingStatus = shipping?.CurrentStatus ?? -1;

                return PartialView("_ChiTietDonHangPartial", orderDetails);
            }
            catch
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError,
                    "Lỗi khi tải chi tiết đơn hàng.");
            }
        }
    }
}