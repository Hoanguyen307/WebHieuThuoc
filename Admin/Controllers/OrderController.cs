using DAL;
using Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Models.Order;
using static Models.Post;

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
            var pagedList = processes.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);

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
        public JsonResult UpdateStatus(int id, string status)
        {
            try
            {
                var result = new Order_DAL().ToggleStatus(id, CurrentUserName, status);
                if (result)
                {
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
            var latestOrderId = db.Orders.Max(o => o.ID); // ID lớn nhất hiện tại
            return Json(new { latestOrderId = latestOrderId }, JsonRequestBehavior.AllowGet);
        }

    }
}