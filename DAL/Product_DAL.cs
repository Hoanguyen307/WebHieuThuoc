using Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using static Models.Product;
using static Models.ProductReview;

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
                param.Add("@ProductCategoryId", filter.ProductCategoryId);
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
                param.Add("@ProductCategoryId", obj.ProductCategoryId);
                param.Add("@Name", obj.Name);
                param.Add("@Slug", obj.Slug);
                param.Add("@Description", obj.Description);
                param.Add("@Price", obj.Price);
                param.Add("@SalePrice", obj.SalePrice);
                param.Add("@Image", obj.Image);
                param.Add("@IsActive", obj.IsActive);
                param.Add("@IsFeatured", obj.IsFeatured);
                param.Add("@Tag", obj.Tags);
                param.Add("@BrandId", obj.BrandId);
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
                param.Add("@ProductCategoryId", obj.ProductCategoryId);
                param.Add("@Name", obj.Name);
                param.Add("@Slug", obj.Slug);
                param.Add("@Description", obj.Description);
                param.Add("@Price", obj.Price);
                param.Add("@SalePrice", obj.SalePrice);
                param.Add("@Image", obj.Image);
                param.Add("@IsActive", obj.IsActive);
                param.Add("@IsFeatured", obj.IsFeatured);
                param.Add("@Tag", obj.Tags);
                param.Add("@BrandId", obj.BrandId);
                param.Add("@UpdatedBy", obj.UpdatedBy);
                return Connection.getConnection().Execute("sp_Product_Update", param, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }
        public bool Update_IsActive(int Id, bool isActive)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", Id);
                param.Add("@IsActive", isActive);
                Connection.getConnection().Execute("sp_Product_Update_IsActive", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
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

        public List<Product> Select_Published(ProductFilter filter, string SortOrder)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ProductCategoryId", filter.ProductCategoryId);
                param.Add("@BrandId", filter.BrandId);
                param.Add("@SortOrder", SortOrder);

                var result = SqlMapper.Query<Product>(Connection.getConnection(), "sp_Product_GetPublished",
                               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                return new List<Product>();
            }
        }
        public List<Product> Search(string SearchString)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@SearchString", SearchString);

                var result = SqlMapper.Query<Product>(Connection.getConnection(), "sp_Product_SearchOnly",
                               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                return new List<Product>();
            }
        }
        public List<Product> Select_TopSelling(int limit = 8)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Limit", limit);
                var result = SqlMapper.Query<Product>(Connection.getConnection(), "sp_Product_GetTopSelling",
                               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                // Xử lý lỗi
                return new List<Product>();
            }
        }
        public List<Product> Select_GetLatest(int limit = 8)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Limit", limit);
                var result = SqlMapper.Query<Product>(Connection.getConnection(), "sp_Product_GetLatest",
                               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                // Xử lý lỗi
                return new List<Product>();
            }
        }
        public List<ThuongHieu> Select_Brands_All()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var result = SqlMapper.Query<ThuongHieu>(Connection.getConnection(), "sp_GetBrands",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<Product> Select_Product_ByBrand(int brandId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@BrandId", brandId);
                var result = SqlMapper.Query<Product>(Connection.getConnection(), "sp_GetProducts_ByBrand",
                               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                return new List<Product>();
            }
        }

        
    }
}
