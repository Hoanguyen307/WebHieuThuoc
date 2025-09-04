using Common;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.DichVu;
using static Models.Product;

namespace DAL
{
    public class DichVu_DAL
    {
        public List<DichVu> Select_All(DichVuFilter filter)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Name", filter.Name);
                param.Add("@MinPrice", filter.MinPrice);
                param.Add("@MaxPrice", filter.MaxPrice);
                var result = SqlMapper.Query<DichVu>(
                    Connection.getConnection(),
                    "sp_DichVu_GetAll", param,
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DichVu SelectById(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", ID);
                var model = SqlMapper.Query<DichVu>(Connection.getConnection(), "sp_DichVu_GetById", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Insert(DichVu obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@DanhMucId", obj.DanhMucId);
                param.Add("@TenDichVu", obj.TenDichVu);
                param.Add("@Gia", obj.Gia);
                param.Add("@ThoiGian", obj.ThoiGian);
                param.Add("@MoTa", obj.MoTa);
                param.Add("@HinhAnh", obj.HinhAnh);
                param.Add("@CreatedBy", obj.CreatedBy);

                return Connection.getConnection().Execute(
                    "sp_DichVu_Insert",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int Update(DichVu obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", obj.Id);
                param.Add("@DanhMucId", obj.DanhMucId);
                param.Add("@TenDichVu", obj.TenDichVu);
                param.Add("@Gia", obj.Gia);
                param.Add("@ThoiGian", obj.ThoiGian);
                param.Add("@MoTa", obj.MoTa);
                param.Add("@HinhAnh", obj.HinhAnh);
                param.Add("@UpdatedBy", obj.UpdatedBy);

                return Connection.getConnection().Execute(
                    "sp_DichVu_Update",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
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
                Connection.getConnection().Execute("sp_DichVu_Delete", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
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
                Connection.getConnection().Execute("sp_DichVu_Update_IsActive", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<DichVu> Select_Published(DichVuFilter filter, string SortOrder)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@DanhMucId", filter.DanhMucId);
                param.Add("@SortOrder", SortOrder);

                var result = SqlMapper.Query<DichVu>(Connection.getConnection(), "sp_DichVu_GetPublished",
                               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                return new List<DichVu>();
            }
        }
        public List<DichVu> Select_GetLatest(int limit = 8)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Limit", limit);
                var result = SqlMapper.Query<DichVu>(Connection.getConnection(), "sp_DichVu_GetLatest",
                               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                // Xử lý lỗi
                return new List<DichVu>();
            }
        }
    }
}
