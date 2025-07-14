using Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace DAL
{
    public class Order_DAL
    {
        public List<Order> Select_Order_All()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var result = SqlMapper.Query<Order>(Connection.getConnection(), "sp_GetOrders_Admin",
                    param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all orders", ex);
            }
        }
        public Order GetOrderDetails(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderId", ID);
                var model = SqlMapper.Query<Order>(Connection.getConnection(), "sp_GetOrderDetails", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving order with ID {ID}", ex);
            }
        }
        public Order GetOrderDetails_ByCustomer(int OrderId, int CustomerId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderId", OrderId);
                param.Add("@CustomerId", CustomerId);
                var model = SqlMapper.Query<Order>(Connection.getConnection(), "sp_GetOrderDetails_ByCustomer", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
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
        public int Insert(Order obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@OrderCode", obj.OrderCode);
                param.Add("@CustomerId", obj.CustomerId);
                param.Add("@Status", obj.Status);
                param.Add("@Note", obj.Note);
                param.Add("@TotalAmount", obj.TotalAmount);
                param.Add("@CreatedBy", obj.CreatedBy);
                return Connection.getConnection().Execute("sp_Order_Insert", param, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return 0;
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
