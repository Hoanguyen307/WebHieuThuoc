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

                var result = conn.Query<OrderDetail, Product, OrderDetail>(
                    "sp_GetOrderDetails",
                    (order, product) =>
                    {
                        OrderDetail orderEntry;

                        if (!lookup.TryGetValue(order.OrderCode, out orderEntry))
                        {
                            orderEntry = order;
                            orderEntry.listProduct = new List<Product>();
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
                    splitOn: "ProductId" // cột đầu tiên thuộc về Product
                );

                return result.FirstOrDefault();
            }
        }

        public OrderDetail GetOrderDetails_ByCustomer(int OrderId, int CustomerId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderId", OrderId);
                param.Add("@CustomerId", CustomerId);
                var model = SqlMapper.Query<OrderDetail>(Connection.getConnection(), "sp_GetOrderDetails_ByCustomer", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return model;
            }
            catch (Exception)
            {
                throw ;
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

                // Tạo DataTable tương ứng OrderDetailType (ProductId, Quantity, UnitPrice, Discount)
                var dt = new DataTable();
                dt.Columns.Add("ProductId", typeof(int));
                dt.Columns.Add("Quantity", typeof(int));
                dt.Columns.Add("UnitPrice", typeof(decimal));
                dt.Columns.Add("Discount", typeof(decimal));

                if (details != null)
                {
                    foreach (var d in details)
                    {
                        var unitPrice = d.UnitPrice;
                        var discount = d.Discount;
                        dt.Rows.Add(d.ProductId, d.Quantity, unitPrice, discount);
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
        public bool ToggleStatus(int ID, string TenNguoiThucHien, string newStatus)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderId", ID);
                param.Add("@UpdatedBy", TenNguoiThucHien);
                param.Add("@Status", newStatus);

                Connection.getConnection().Execute("sp_OrderStatus_Update", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
