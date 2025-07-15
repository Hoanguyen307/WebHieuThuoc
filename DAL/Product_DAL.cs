using Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using static Models.Product;

namespace DAL
{
    public class Product_DAL
    {
        public List<Product> Select_Product_All(ProductFilter filter)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Name", filter.Name);
                param.Add("@Month", filter.Month);
                param.Add("@Year", filter.Year);
                param.Add("@CategoryId", filter.CategoryId);
                param.Add("@MinPrice", filter.MinPrice);
                param.Add("@MaxPrice", filter.MaxPrice);
                var result = SqlMapper.Query<Product>(Connection.getConnection(), "sp_Product_GetAll",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Product SelectById(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", ID);
                var model = SqlMapper.Query<Product>(Connection.getConnection(), "sp_Product_GetById", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Insert(Product obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CategoryId", obj.CategoryId);
                param.Add("@Name", obj.Name);
                param.Add("@Slug", obj.Slug);
                param.Add("@Description", obj.Description);
                param.Add("@Price", obj.Price);
                param.Add("@SalePrice", obj.SalePrice);
                param.Add("@Image", obj.Image);
                param.Add("@Quantity", obj.Quantity);
                param.Add("@IsActive", obj.IsActive);
                param.Add("@IsFeatured", obj.IsFeatured);
                param.Add("@Tag", obj.Tags);
                param.Add("@CreatedBy", obj.CreatedBy);
                return Connection.getConnection().Execute("sp_Product_Insert", param, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }
        public int Update(Product obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", obj.Id);
                param.Add("@CategoryId", obj.CategoryId);
                param.Add("@Name", obj.Name);
                param.Add("@Slug", obj.Slug);
                param.Add("@Description", obj.Description);
                param.Add("@Price", obj.Price);
                param.Add("@SalePrice", obj.SalePrice);
                param.Add("@Image", obj.Image);
                param.Add("@Quantity", obj.Quantity);
                param.Add("@IsActive", obj.IsActive);
                param.Add("@IsFeatured", obj.IsFeatured);
                param.Add("@Tag", obj.Tags);
                param.Add("@UpdatedBy", obj.UpdatedBy);
                return Connection.getConnection().Execute("sp_Product_Update", param, commandType: System.Data.CommandType.StoredProcedure);
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
                Connection.getConnection().Execute("sp_Product_Delete", param, commandType: System.Data.CommandType.StoredProcedure);
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
