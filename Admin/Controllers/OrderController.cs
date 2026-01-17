using DAL;
using Models;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Models.Order;
using static Models.Post;
using System.Configuration;

namespace Admin.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class OrderController : Controller
    {
        private DBConnect db = new DBConnect();
        public string CurrentUserName
        {
            get
            {
                var kh = Session["Login"] as Models.LoginViewModel;
                if (kh == null)
                {
                    return null;
                }
                return kh.UserName;
            }
        }
        // GET: Order
        public ActionResult Index(string searchString, int? Month, int? Year, string Status, int page = 1, int pageSize = 10)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var year = Enumerable.Range(currentYear - 5, 11).Select(y => new { Id = y, Name = y.ToString() }).ToList();
            ViewBag.Years = new SelectList(year, "Id", "Name", Year);

            var months = Enumerable.Range(1, 12).Select(m => new { Id = m, Name = $"Tháng {m}" }).ToList();
            ViewBag.Months = new SelectList(months, "Id", "Name", Month);

            return View();
        }

        [HttpGet]
        public JsonResult GetDonHang(string searchString, int? Month, int? Year, string Status, int page = 1, int pageSize = 10)
        {
            if (Month == 0) Month = null;
            if (Year == 0) Year = null;
            searchString = string.IsNullOrWhiteSpace(searchString) ? null : searchString;
            Status = string.IsNullOrWhiteSpace(Status) ? null : Status;

            OrderFilter filter = new OrderFilter
            {
                Keyword = string.IsNullOrWhiteSpace(searchString) ? null : searchString,
                Month = Month,
                Year = Year,
                Status = Status
            };

            var processes = new Order_DAL().Select_Order_All(filter);
            var pagedList = processes.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);

            return Json(new
            {
                items = pagedList.ToList(),
                totalCount = pagedList.TotalItemCount,
                currentPage = pagedList.PageNumber,
                pageSize = pagedList.PageSize
            }, JsonRequestBehavior.AllowGet);
        }

        /*public JsonResult LichSuDonHang(int id)
        {
            try
            {
                List<Order> lstmodel = new Order_DAL().LichSu_DonHang(id);
                if (lstmodel != null && lstmodel.Any())
                {
                    var data = lstmodel.Select(x => new
                    {
                        x.PositionName,
                        TuNgay = x.TuNgay?.ToString("yyyy-MM-dd"),
                        DenNgay = x.DenNgay?.ToString("yyyy-MM-dd")
                    }).ToList();

                    return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = true, data = new List<object>() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }*/

        public ActionResult ChiTietDonHang(int id)
        {
            var products = db.Products
                             .Select(p => new { p.ThuocId, p.TenThuoc })
                             .ToList();
            ViewBag.Products = JsonConvert.SerializeObject(products);

            var order = new Order_DAL().GetOrderDetails(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return PartialView("ChiTietDonHang", order);
        }

        [HttpPost]
        public JsonResult DeleteAccount(int Id, string TenNguoiXoa)
        {
            try
            {
                var result = new Order_DAL().Delete(Id, TenNguoiXoa);
                if (result)
                {
                    return Json(new { code = 200, msg = "Xóa thành công" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Xóa thất bại" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDropdownData()
        {
            try
            {
                var currentYear = DateTime.Now.Year;
                var currentMonth = DateTime.Now.Month;
                var year = Enumerable.Range(currentYear - 5, 11).Select(y => new { Id = y, Name = y.ToString() }).ToList();

                var months = Enumerable.Range(1, 12).Select(m => new { Id = m, Name = $"Tháng {m}" }).ToList();

                return Json(new
                {
                    success = true,
                    months = months,
                    year = year,
                    currentYear = currentYear,
                    currentMonth = currentMonth
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public JsonResult UpdateOrderPrescription(int orderId, List<OrderDetail> details)
        {
            try
            {
                decimal newTotal = details.Sum(x => x.Quantity * x.UnitPrice);

                bool success = new Order_DAL().UpdateOrderItems(orderId, details, newTotal);

                if (success)
                {
                    var orderInfo = new Order_DAL().GetOrderDetails(orderId);

                    if (orderInfo != null && !string.IsNullOrEmpty(orderInfo.Email))
                    {
                        string confirmationLink = "http://nguyentiendat18032003.id.vn/KhachHang/ChiTietDonHang/" + orderId;
                        string subject = $"[Báo giá] Đơn thuốc #{orderInfo.OrderCode} đã có giá";
                        string body = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; padding: 20px; border: 1px solid #ddd;'>
                        <h2 style='color: #28a745;'>Thông báo báo giá đơn thuốc</h2>
                        <p>Chào <b>{orderInfo.FullName}</b>,</p>
                        <p>Dược sĩ của <b>Nhà thuốc Thanh Tứ</b> đã xem đơn thuốc và soạn danh sách thuốc cho bạn.</p>
                        <div style='background: #f8f9fa; padding: 15px; margin: 20px 0;'>
                            <p>Mã đơn hàng: <b>#{orderInfo.OrderCode}</b></p>
                            <p>Trạng thái: <b style='color: orange;'>Đã báo giá</b></p>
                            <p style='font-size: 18px;'>Tổng tiền thanh toán: <b style='color: red;'>{newTotal:N0} ₫</b></p>
                        </div>
                        <p>Vui lòng nhấn vào nút bên dưới để xem danh sách thuốc chi tiết và xác nhận đơn hàng:</p>
                        <div style='text-align: center; margin-top: 20px;'>
                        <div style='text-align: center; margin-top: 20px;'>
                            <a href='{confirmationLink}' style='background-color: #28a745; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;'>XEM CHI TIẾT & XÁC NHẬN</a>
                        </div>
                        <hr/>
                        <p style='font-size: 12px; color: #888;'>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
                    </div>";

                        Task.Run(() => SendEmailNotification(orderInfo.Email, subject, body));
                    }

                    return Json(new { code = 200, msg = "Cập nhật đơn thuốc và gửi báo giá thành công" });
                }
                return Json(new { code = 500, msg = "Cập nhật thất bại" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateStatus(int id, string status, string carrierName)
        {
            try
            {
                string TenNguoiThucHien = User?.Identity?.Name ?? "Unknown";
                var result = new Order_DAL().ToggleStatus(id, TenNguoiThucHien, status, carrierName);
                if (result)
                {
                    var orderInfo = new Order_DAL().GetOrderDetails(id);

                    if (orderInfo != null && !string.IsNullOrEmpty(orderInfo.Email))
                    {
                        string subject = $"Cập nhật đơn hàng #{orderInfo.OrderCode} - Nhà thuốc Thanh Tứ";

                        string statusColor = status == "Hủy" ? "red" : "blue";
                        string body = $@"
                            <div style='font-family: Segoe UI, Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; border: 1px solid #eee; padding: 20px;'>
                                <h2 style='color: #444;'>Thông báo trạng thái đơn hàng</h2>
                                <p>Chào <b>{orderInfo.FullName}</b>,</p>
                                <p>Đơn hàng <b>#{orderInfo.OrderCode}</b> của bạn đã được cập nhật trạng thái mới:</p>
                                <div style='background: #f9f9f9; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                                    <p style='margin: 5px 0;'>Trạng thái: <span style='color: {statusColor}; font-weight: bold;'>{status}</span></p>
                                    <p style='margin: 5px 0;'>Tổng tiền: <b>{orderInfo.TotalAmount:N0} ₫</b></p>
                                    {(string.IsNullOrEmpty(carrierName) ? "" : $"<p style='margin: 5px 0;'>Đơn vị vận chuyển: {carrierName}</p>")}
                                </div>
                                <p>Cảm ơn bạn đã tin tưởng lựa chọn <b>Nhà thuốc Thanh Tứ</b>!</p>
                                <hr style='border: 0; border-top: 1px solid #eee;' />
                                <p style='font-size: 12px; color: #888;'>Đây là email tự động, vui lòng không phản hồi email này.</p>
                            </div>";

                        // Gửi mail bất đồng bộ
                        Task.Run(() => SendEmailNotification(orderInfo.Email, subject, body));
                    }
                    return Json(new { code = 200, msg = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Cập nhật thất bại" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 550, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult SearchProductAdmin(string term)
        {
            var products = db.Products
                .Where(p => p.TenThuoc.Contains(term))
                .Select(p => new {
                    ThuocId = p.ThuocId,
                    TenThuoc = p.TenThuoc,
                    GiaBan = p.GiaBan,
                    GiaGoc = p.GiaGoc
                }).Take(10).ToList();
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetNewOrders(int lastOrderId = 0)
        {
            try
            {
                // Lấy danh sách đơn mới hơn so với lastOrderId
                var newOrders = new Order_DAL().Select_Order_All(new OrderFilter())
                    .Where(o => o.ID > lastOrderId)
                    .OrderBy(o => o.ID)
                    .Select(o => new
                    {
                        o.ID,
                        o.OrderCode,
                        o.CustomerName,
                        o.TotalAmount,
                        CreatedDate = o.CreatedDate,
                        o.Status
                    })
                    .ToList();

                var newestId = newOrders.Any() ? newOrders.Max(x => x.ID) : lastOrderId;

                return Json(new
                {
                    hasNew = newOrders.Any(),
                    newCount = newOrders.Count,
                    newestOrderId = newestId,
                    orders = newOrders
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { hasNew = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetLatestOrderId()
        {
            var latestOrderId = db.Orders.Max(o => o.ID); 
            return Json(new { latestOrderId = latestOrderId }, JsonRequestBehavior.AllowGet);
        }

        private bool SendEmailNotification(string toEmail, string subject, string body)
        {
            try
            {
                string fromEmail = ConfigurationManager.AppSettings["Mail_From"];
                string fromPassword = ConfigurationManager.AppSettings["Mail_Password"];
                string host = ConfigurationManager.AppSettings["Mail_Host"];
                int port = int.Parse(ConfigurationManager.AppSettings["Mail_Port"]);

                var fromAddress = new MailAddress(fromEmail, "Nhà thuốc Thanh Tứ");
                var toAddress = new MailAddress(toEmail);

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                    EnableSsl = true,
                    Timeout = 20000,
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(message);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}