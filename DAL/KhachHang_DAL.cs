using Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using static Models.Product;
using static Models.KhachHang;
using static Models.PointsModel;

namespace DAL
{
    public class KhachHang_DAL
    {
        public List<KhachHang> Select_KhachHang_All(KhachHangFilter filter)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@FullName", filter.FullName);
                param.Add("@Month", filter.Month);
                param.Add("@Year", filter.Year);
                var result = SqlMapper.Query<KhachHang>(Connection.getConnection(), "sp_Customers_GetAll",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<KhachHang> Select_KhachHang_GetAll()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var result = SqlMapper.Query<KhachHang>(Connection.getConnection(), "sp_Customers_SelectAll",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public KhachHang DangNhap(string tendangnhap, string matkhau)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Identifier", tendangnhap);
                param.Add("@Password", matkhau);
                var model = SqlMapper.Query<KhachHang>(Connection.getConnection(), "sp_Customer_Login", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return model;
            }
            catch
            {
                throw;
            }
        }
        public KhachHang SelectById(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", ID);
                var model = SqlMapper.Query<KhachHang>(Connection.getConnection(), "sp_Customers_GetById", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Order LichSu_DonHang(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CustomerId", ID);
                var model = SqlMapper.Query<Order>(Connection.getConnection(), "sp_GetOrders_ByCustomer", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving order with ID {ID}", ex);
            }
        }

        public int Insert(KhachHang obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@FullName", obj.FullName);
                param.Add("@Gender", obj.Gender);
                param.Add("@BirthDate", obj.BirthDate);
                param.Add("@Phone", obj.Phone);
                param.Add("@Email", obj.Email);
                param.Add("@Address", obj.Address);
                param.Add("@PasswordHash", obj.PasswordHash);
                param.Add("@CreatedBy", obj.CreatedBy);
                return Connection.getConnection().Execute("sp_Customer_Insert", param, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }
        public int Update(KhachHang obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", obj.Id);
                param.Add("@FullName", obj.FullName);
                param.Add("@Gender", obj.Gender);
                param.Add("@BirthDate", obj.BirthDate);
                param.Add("@Phone", obj.Phone);
                param.Add("@Email", obj.Email);
                param.Add("@Address", obj.Address);
                param.Add("@PasswordHash", obj.PasswordHash);
                param.Add("@UpdatedBy", obj.UpdatedBy);
                return Connection.getConnection().Execute("sp_Customer_Update", param, commandType: System.Data.CommandType.StoredProcedure);
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
                Connection.getConnection().Execute("sp_Customer_Delete", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public bool ToggleStatus(int ID, string TenNguoiThucHien, string LyDo)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", ID);
                param.Add("@LockBy", TenNguoiThucHien);
                param.Add("@LockReason", LyDo);

                Connection.getConnection().Execute("sp_Customers_ToggleStatus", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<PointsHistory> LichSu_Diem(int customerId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CustomerId", customerId);

                var history = SqlMapper.Query<PointsHistory>(
                    Connection.getConnection(),
                    "sp_GetPointsHistory_ByCustomer",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();

                return history;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving point history for customer {customerId}", ex);
            }
        }

    }
}
