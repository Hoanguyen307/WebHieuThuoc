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
    public class ProductCategory_DAL
    {
        public List<ProductCategory> Select_Category_All()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var result = SqlMapper.Query<ProductCategory>(Connection.getConnection(), "sp_ProductCategories_GetAll",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public ProductCategory SelectById(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", ID);
                var model = SqlMapper.Query<ProductCategory>(Connection.getConnection(), "SP_ProductCategories_GetById", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Insert(ProductCategory obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Name", obj.Name);
                param.Add("@Slug", obj.Slug);
                param.Add("@Description", obj.Description);
                param.Add("@IsActive", obj.IsActive);
                param.Add("@CategoryId", obj.CategoryId);
                param.Add("@CreatedBy", obj.CreatedBy);
                return Connection.getConnection().Execute("sp_ProductCategories_Insert", param, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public int Update(ProductCategory obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", obj.Id);
                param.Add("@Name", obj.Name);
                param.Add("@Slug", obj.Slug);
                param.Add("@Description", obj.Description);
                param.Add("@IsActive", obj.IsActive);
                param.Add("@CategoryId", obj.CategoryId);
                param.Add("@UpdatedBy", obj.UpdatedBy);
                return Connection.getConnection().Execute("sp_ProductCategories_Update", param, commandType: System.Data.CommandType.StoredProcedure);
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
                Connection.getConnection().Execute("sp_ProductCategories_Delete", param, commandType: System.Data.CommandType.StoredProcedure);
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
