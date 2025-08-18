using Common;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Voucher_DAL
    {
        public List<Voucher> Select_Voucher_All()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var result = SqlMapper.Query<Voucher>(Connection.getConnection(), "sp_Vouchers_GetAll",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public Voucher SelectById(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", ID);
                var model = SqlMapper.Query<Voucher>(Connection.getConnection(), "sp_Vouchers_GetById", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Insert(Voucher obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Code", obj.Code);
                param.Add("@Percentage", obj.Percentage);
                param.Add("@DiscountValue", obj.DiscountValue);
                param.Add("@Quantity", obj.Quantity);
                param.Add("@StartDate", obj.StartDate);
                param.Add("@EndDate", obj.EndDate);
                param.Add("@Description", obj.Description);
                return Connection.getConnection().Execute("sp_Voucher_Insert", param, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public int Update(Voucher obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", obj.Id);
                param.Add("@Code", obj.Code);
                param.Add("@Percentage", obj.Percentage);
                param.Add("@DiscountValue", obj.DiscountValue);
                param.Add("@Quantity", obj.Quantity);
                param.Add("@StartDate", obj.StartDate);
                param.Add("@EndDate", obj.EndDate);
                param.Add("@Description", obj.Description);
                param.Add("@IsActive", obj.IsActive);
                return Connection.getConnection().Execute("sp_Voucher_Update", param, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public bool Delete(int ID, string TenNguoiXoa)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", ID);
                param.Add("@DeletedBy", TenNguoiXoa);
                Connection.getConnection().Execute("sp_Voucher_Delete", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}
