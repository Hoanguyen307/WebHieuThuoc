using Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace DAL
{
    public class Category_DAL
    {
        public List<Category> Select_Category_All()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var result = SqlMapper.Query<Category>(Connection.getConnection(), "Categories_GetAll",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<Menu> Select_Menu_All()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var result = SqlMapper.Query<Menu>(Connection.getConnection(), "sp_Menus_GetAll",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public Category SelectById(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", ID);
                var model = SqlMapper.Query<Category>(Connection.getConnection(), "Categories_GetById", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Category> Select_ByMenuId(int menuId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@MenuId", menuId);
                var model = SqlMapper.Query<Category>(Connection.getConnection(), "sp_Categories_GetByMenuId", param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Insert(Category obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Name", obj.Name);
                param.Add("@Slug", obj.Slug);
                param.Add("@Description", obj.Description);
                param.Add("@DisplayOrder", obj.DisplayOrder);
                param.Add("@IsActive", obj.IsActive);
                param.Add("@CreatedBy", obj.CreatedBy);
                return Connection.getConnection().Execute("Categories_Insert", param, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public int Update(Category obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", obj.ID);
                param.Add("@Name", obj.Name);
                param.Add("@Slug", obj.Slug);
                param.Add("@Description", obj.Description);
                param.Add("@DisplayOrder", obj.DisplayOrder);
                param.Add("@IsActive", obj.IsActive);
                param.Add("@UpdatedBy", obj.UpdatedBy);
                return Connection.getConnection().Execute("Categories_Update", param, commandType: System.Data.CommandType.StoredProcedure);
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
                Connection.getConnection().Execute("Categories_Delete", param, commandType: System.Data.CommandType.StoredProcedure);
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
