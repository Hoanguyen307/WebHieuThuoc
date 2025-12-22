using DAL;
using Models;
using System;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class ShippingController : Controller
    {
        private readonly Shipping_DAL _dal = new Shipping_DAL();

        public string CurrentUserName
        {
            get
            {
                var kh = Session["Login"] as Models.LoginViewModel;
                return kh?.UserName ?? "Unknown";
            }
        }

        // ================== INDEX ========================
        public ActionResult Index()
        {
            ViewBag.DeliveryServices = _dal.GetDeliveryServices();
            ViewBag.Warehouses = _dal.GetWarehouses();
            return View();
        }

        // ================== GET SHIPPING ORDERS ========================
        [HttpGet]
        public JsonResult GetShippingOrders(string keyword = null, int? status = null, int? deliveryServiceId = null, int? driverId = null, int? warehouseId = null, DateTime? fromDate = null, DateTime? toDate = null)

        {
            try
            {
                var list = _dal.GetAllShippingOrders(keyword, status, deliveryServiceId, driverId, warehouseId, fromDate, toDate);
                return Json(new { code = 200, data = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetDeliveryServices()
        {
            try
            {
                var list = _dal.GetDeliveryServices();
                return Json(new { code = 200, data = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        // ================== ASSIGN DVVC / DRIVER / WAREHOUSE =============
        [HttpPost]
        public JsonResult Assign(int shippingOrderId, int? deliveryServiceId, int? driverId, int? warehouseId)
        {
            try
            {
                var ok = _dal.UpdateShippingOrder(shippingOrderId, deliveryServiceId, driverId, warehouseId);
                if (!ok) return Json(new { code = 500, msg = "Không thể cập nhật" });

                return Json(new { code = 200, msg = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message });
            }
        }

        // ================== UPDATE STATUS =========================
        [HttpPost]
        public JsonResult UpdateStatus(int shippingOrderId, int status, string location, string note)
        {
            try
            {
                _dal.UpdateShippingStatus(shippingOrderId, status, location, note);
                return Json(new { code = 200, msg = "Cập nhật trạng thái thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = ex.Message });
            }
        }

        // ================== GET HISTORY =========================
        [HttpGet]
        public JsonResult GetHistory(int shippingOrderId)
        {
            try
            {
                var list = _dal.GetShippingHistory(shippingOrderId);
                return Json(new { code = 200, data = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ================== GET DRIVERS =========================
        [HttpGet]
        public JsonResult GetDrivers(int deliveryServiceId)
        {
            try
            {
                var list = _dal.GetDriversByService(deliveryServiceId);
                return Json(new { code = 200, data = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ================== GET WAREHOUSE =======================
        [HttpGet]
        public JsonResult GetWarehouses()
        {
            try
            {
                var list = _dal.GetWarehouses();
                return Json(new { code = 200, data = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
