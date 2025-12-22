using Common;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Shipping_DAL
    {
        public bool UpdateShippingOrder(int shippingOrderId, int? deliveryServiceId, int? driverId, int? warehouseId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ShippingOrderId", shippingOrderId);
                param.Add("@DeliveryServiceId", deliveryServiceId);
                param.Add("@DriverId", driverId);
                param.Add("@WarehouseId", warehouseId);

                Connection.getConnection().Execute(
                    "sp_UpdateShippingOrder",
                    param,
                    commandType: CommandType.StoredProcedure);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateShippingStatus(int shippingOrderId, int status, string location, string note)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ShippingOrderId", shippingOrderId);
                param.Add("@Status", status);
                param.Add("@Location", location);
                param.Add("@Note", note);

                Connection.getConnection().Execute(
                    "sp_UpdateShippingStatus",
                    param,
                    commandType: CommandType.StoredProcedure);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public ShippingOrder GetShippingByOrderId(int orderId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@OrderId", orderId);

                var result = Connection.getConnection().QueryFirstOrDefault<ShippingOrder>(
                    "sp_GetShippingByOrderId",
                    param,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving ShippingOrder for OrderId={orderId}", ex);
            }
        }
        public List<ShippingStatusHistory> GetShippingHistory(int shippingOrderId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ShippingOrderId", shippingOrderId);

                var list = Connection.getConnection().Query<ShippingStatusHistory>(
                    "sp_GetShippingHistory",
                    param,
                    commandType: CommandType.StoredProcedure).ToList();

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving shipping history id={shippingOrderId}", ex);
            }
        }
        public List<DeliveryService> GetDeliveryServices()
        {
            try
            {
                var list = Connection.getConnection().Query<DeliveryService>(
                    "sp_GetDeliveryServices",
                    commandType: CommandType.StoredProcedure).ToList();

                return list;
            }
            catch
            {
                return new List<DeliveryService>();
            }
        }
        public List<Driver> GetDriversByService(int deliveryServiceId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@DeliveryServiceId", deliveryServiceId);

                var list = Connection.getConnection().Query<Driver>(
                    "sp_GetDriversByService",
                    param,
                    commandType: CommandType.StoredProcedure).ToList();

                return list;
            }
            catch
            {
                return new List<Driver>();
            }
        }
        public List<ShippingOrderViewModel> GetAllShippingOrders(string keyword, int? status, int? deliveryServiceId, int? driverId, int? warehouseId, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Keyword", keyword);
                param.Add("@Status", status);
                param.Add("@DeliveryServiceId", deliveryServiceId);
                param.Add("@DriverId", driverId);
                param.Add("@WarehouseId", warehouseId);
                param.Add("@FromDate", fromDate);
                param.Add("@ToDate", toDate);

                var list = Connection.getConnection().Query<ShippingOrderViewModel>(
                    "sp_GetShippingOrders",
                    param,
                    commandType: CommandType.StoredProcedure).ToList();

                return list;
            }
            catch
            {
                return new List<ShippingOrderViewModel>();
            }
        }
        public ShippingOrder GetShippingOrderByTracking(string tracking)
        {
            return Connection.getConnection().QueryFirstOrDefault<ShippingOrder>(
                "SELECT * FROM ShippingOrders WHERE TrackingCode = @tracking",
                new { tracking });
        }
        public ShippingOrder GetByOrderId(int orderId)
        {
            return Connection.getConnection().QueryFirstOrDefault<ShippingOrder>(
                "sp_GetShippingByOrderId",
                new { OrderId = orderId },
                commandType: CommandType.StoredProcedure
            );
        }

        public List<KhoHang> GetWarehouses()
        {
            return Connection.getConnection().Query<KhoHang>(
                "SELECT * FROM KhoHang").ToList();
        }
    }
}
