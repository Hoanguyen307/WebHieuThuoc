using Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace DAL
{
    public class Account_DAL
    {
        public User SelectById(int IDnguoidung)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", IDnguoidung);
                var model = SqlMapper.Query<User>(Connection.getConnection(), "sp_GetUserById", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Insert(User obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@UserName", obj.UserName);
                param.Add("@Password", obj.Password);
                param.Add("@FullName", obj.FullName);
                param.Add("@Email", obj.Email);
                param.Add("@Phone", obj.Phone);
                param.Add("@RoleId", obj.RoleId == 0 ? (int?)null : obj.RoleId);
                param.Add("@CreatedBy", obj.CreatedBy);
                return Connection.getConnection().Execute("sp_AddUserCustom", param, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }
        public bool Update(User obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", obj.Id);
                param.Add("@UserName", obj.UserName);
                param.Add("@FullName", obj.FullName);
                param.Add("@Email", obj.Email);
                param.Add("@Phone", obj.Phone);
                param.Add("@RoleId", obj.RoleId == 0 ? (int?)null : obj.RoleId);
                param.Add("@UpdatedBy", obj.UpdatedBy);
                Connection.getConnection().Execute("sp_UpdateUser", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public bool Delete(int IDnguoidung, string TenNguoiXoa)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", IDnguoidung);
                param.Add("@DeletedBy", TenNguoiXoa);
                Connection.getConnection().Execute("sp_DeleteUser", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public List<User> Select_NguoiDung_All()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var result = SqlMapper.Query<User>(Connection.getConnection(), "sp_GetUsers",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public User DangNhap(string tendangnhap, string matkhau)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@UserName", tendangnhap);
                param.Add("@Password", matkhau);
                var model = SqlMapper.Query<User>(Connection.getConnection(), "sp_LoginUser", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return model;
            }
            catch
            {
                throw;
            }
        }
    }
}
