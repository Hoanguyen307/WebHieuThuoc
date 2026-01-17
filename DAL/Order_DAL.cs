using Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using static Models.Order;
using System.Data;

namespace DAL
{
    public class Order_DAL
    {
        public List<Order> Select_Order_All(OrderFilter filter)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Keyword", filter.Keyword);
                param.Add("@Month", filter.Month);
                param.Add("@Year", filter.Year);
                param.Add("@Status", filter.Status);
                var result = SqlMapper.Query<Order>(Connection.getConnection(), "sp_GetOrders_Admin",
                    param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all orders", ex);
            }
        }
        public OrderDetail GetOrderDetails(int orderId)
        {
            using (var conn = Connection.getConnection())
            {
                var lookup = new Dictionary<string, OrderDetail>();

                var result = conn.Query<OrderDetail, OrderDetail, OrderDetail>(
                    "sp_GetOrderDetails",
                    (order, product) =>
                    {
                        if (!lookup.TryGetValue(order.OrderCode, out var orderEntry))
                        {
                            orderEntry = order;
                            orderEntry.listProduct = new List<OrderDetail>();
                            lookup.Add(order.OrderCode, orderEntry);
                        }

                        if (product != null)
                        {
                            orderEntry.listProduct.Add(product); 
                        }

                        return orderEntry;
                    },
                    new { OrderId = orderId },
                    commandType: CommandType.StoredProcedure,
                    splitOn: "ProductId"
                );

                return result.FirstOrDefault();
            }
        }

        public List<OrderDetail> GetOrderDetails_ByCustomer(int OrderId, int CustomerId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderId", OrderId);
                param.Add("@CustomerId", CustomerId);

                var model = SqlMapper.Query<OrderDetail>(Connection.getConnection(), "sp_GetOrderDetails_ByCustomer", param, commandType: System.Data.CommandType.StoredProcedure).ToList();

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Order> LichSu_DonHang(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CustomerId", ID);
                var model = SqlMapper.Query<Order>(Connection.getConnection(), "sp_GetOrders_ByCustomer", param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return model;
            }catch(Exception ex)
            {
                throw new Exception($"Error retrieving order with ID {ID}", ex);
            }
        }
        public int Insert(Order obj, List<OrderDetail> details, int? customerVoucherId = null, decimal usedPoints = 0)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@OrderCode", obj.OrderCode);
                param.Add("@CustomerId", obj.CustomerId);
                param.Add("@Status", obj.Status);
                param.Add("@Note", obj.Note);
                param.Add("@DiaChiId", obj.DiaChiId);
                param.Add("@CreatedBy", obj.CreatedBy);
                param.Add("@PaymentMethod", obj.PaymentMethod ?? "COD");
                param.Add("@OrderType", obj.OrderType);
                param.Add("@HinhAnhDonThuoc", obj.HinhAnhDonThuoc);

                // Tạo DataTable tương ứng OrderDetailType (ProductId, Quantity, UnitPrice, Discount)
                var dt = new DataTable();
                dt.Columns.Add("ProductId", typeof(int)); 
                dt.Columns.Add("SoLuong", typeof(int));    
                dt.Columns.Add("Gia", typeof(decimal));  
                dt.Columns.Add("GiamGia", typeof(decimal));

                if (details != null)
                {
                    foreach (var d in details)
                    {
                        dt.Rows.Add(d.ProductId, d.Quantity, d.UnitPrice, d.Discount);
                    }
                }

                // Table Valued Parameter (tên type phải trùng với DB: OrderDetailType)
                param.Add("@OrderDetails", dt.AsTableValuedParameter("OrderDetailType"));
                param.Add("@CustomerVoucherId", customerVoucherId);
                param.Add("@UsedPoints", usedPoints);

                // Store trả về NewOrderId (SELECT @OrderId AS NewOrderId)
                var conn = Connection.getConnection();
                var newOrderId = conn.QuerySingle<int>(
                    "sp_Order_Insert",
                    param,
                    commandType: CommandType.StoredProcedure);

                return newOrderId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting order with details", ex);
            }
        }
        public decimal GetOrderTotalAmount(int orderId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderId", orderId);

                return Connection.getConnection().QuerySingleOrDefault<decimal>(
                    "SELECT TotalAmount FROM DonHang WHERE Id = @OrderId",
                    param,
                    commandType: System.Data.CommandType.Text);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Delete(int ID, string TenNguoiXoa)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", ID);
                param.Add("@DeletedBy", TenNguoiXoa);
                Connection.getConnection().Execute("sp_Order_Delete", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public bool ToggleStatus(int ID, string TenNguoiThucHien, string newStatus, string carrierName)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderId", ID);
                param.Add("@UpdatedBy", TenNguoiThucHien);
                param.Add("@Status", newStatus);
                param.Add("@CarrierName", carrierName);

                Connection.getConnection().Execute("sp_OrderStatus_Update", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdateOrderItems(int orderId, List<OrderDetail> details, decimal totalAmount)
        {
            using (var conn = Connection.getConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        conn.Execute("DELETE FROM DonHangChiTiet WHERE Order_ID = @OrderId", new { OrderId = orderId }, trans);

                        foreach (var d in details)
                        {
                            conn.Execute(@"INSERT INTO DonHangChiTiet(Order_ID, ProductId, Quantity, UnitPrice, Discount) 
                                   VALUES (@orderId, @ProductId, @Quantity, @UnitPrice, 0)",
                                           new { orderId, d.ProductId, d.Quantity, d.UnitPrice }, trans);
                        }

                        conn.Execute(@"UPDATE DonHang SET TotalAmount = @totalAmount, Status = N'Đã báo giá' 
                               WHERE Id = @orderId", new { totalAmount, orderId }, trans);

                        trans.Commit();
                        return true;
                    }
                    catch
                    {
                        trans.Rollback();
                        return false;
                    }
                }
            }
        }
    }
}
