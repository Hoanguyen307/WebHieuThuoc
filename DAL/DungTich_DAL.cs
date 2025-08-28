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
    public class DungTich_DAL
    {
        public List<DungTichSanPham> Select_DungTichSP_All()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var result = SqlMapper.Query<DungTichSanPham>(Connection.getConnection(), "sp_DungTichSanPham_GetAll",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<DungTich> Select_DungTich_All()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var result = SqlMapper.Query<DungTich>(Connection.getConnection(), "sp_DungTich_GetAll",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<DungTichSanPham> SelectByProductId(int productId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ProductId", productId);
                var model = SqlMapper.Query<DungTichSanPham>(Connection.getConnection(), "sp_GetProductDungTich_ByProductId", param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DungTichSanPham SelectById(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", ID);
                var model = SqlMapper.Query<DungTichSanPham>(Connection.getConnection(), "sp_ProductDungTich_GetById", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Insert(DungTichSanPham obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ProductId", obj.ProductId);
                param.Add("@DungTichId", obj.DungTichId);
                param.Add("@Gia", obj.Gia);
                param.Add("@SalePrice", obj.SalePrice);
                param.Add("@SoLuong", obj.SoLuong);
                return Connection.getConnection().Execute("sp_DungTichSanPham_Insert", param, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public int Update(DungTichSanPham obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", obj.Id);
                param.Add("@DungTichId", obj.DungTichId);
                param.Add("@Gia", obj.Gia);
                param.Add("@SalePrice", obj.SalePrice);
                param.Add("@SoLuong", obj.SoLuong);
                return Connection.getConnection().Execute("sp_DungTichSanPham_Update", param, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public bool Delete(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", ID);
                Connection.getConnection().Execute("sp_DungTichSanPham_Delete", param, commandType: System.Data.CommandType.StoredProcedure);
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
