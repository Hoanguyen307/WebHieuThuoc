using Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using static Models.Post;

namespace DAL
{
    public class BaiViet_DAL
    {
        public List<Post> Select_ForUser(string loaiDanhMuc, int pageIndex, int pageSize, out int totalCount)
        {
            totalCount = 0;
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@LoaiDanhMuc", loaiDanhMuc);
                param.Add("@PageIndex", pageIndex);
                param.Add("@PageSize", pageSize);

                using (var db = Connection.getConnection())
                {
                    using (var multi = db.QueryMultiple("sp_BaiViet_GetForUser", param, commandType: System.Data.CommandType.StoredProcedure))
                    {
                        var list = multi.Read<Post>().ToList();
                        totalCount = multi.Read<int>().FirstOrDefault();
                        return list;
                    }
                }
            }
            catch (Exception)
            {
                return new List<Post>();
            }
        }

        public List<Post> SelectTopNew(int top = 3)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Top", top);
                var result = SqlMapper.Query<Post>(Connection.getConnection(), "sp_BaiViet_GetTopNew",
                             param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                return new List<Post>();
            }
        }
        public List<Post> Select_BaiViet_All(PostFilter filter)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@TieuDe", filter.TieuDe);
                param.Add("@Month", filter.Month);
                param.Add("@Year", filter.Year);
                var result = SqlMapper.Query<Post>(Connection.getConnection(), "sp_BaiViet_GetAll",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Post SelectById(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", ID);
                var model = SqlMapper.Query<Post>(Connection.getConnection(), "sp_BaiViet_GetById", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Insert(Post obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@TieuDe", obj.TieuDe);
                param.Add("@NoiDung", obj.NoiDung);
                param.Add("@AnhDaiDien", obj.AnhDaiDien);
                param.Add("@LoaiDanhMuc", obj.LoaiDanhMuc);
                param.Add("@CreatedBy", obj.CreatedBy);
                return Connection.getConnection().Execute("sp_BaiViet_Insert", param, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }
        public int Update(Post obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", obj.Id);
                param.Add("@TieuDe", obj.TieuDe);
                param.Add("@NoiDung", obj.NoiDung);
                param.Add("@AnhDaiDien", obj.AnhDaiDien);
                param.Add("@LoaiDanhMuc", obj.LoaiDanhMuc);
                param.Add("@UpdatedBy", obj.UpdatedBy);
                return Connection.getConnection().Execute("sp_BaiViet_Update", param, commandType: System.Data.CommandType.StoredProcedure);
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
                Connection.getConnection().Execute("sp_BaiViet_Delete", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public bool ToggleStatus(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", ID);

                Connection.getConnection().Execute("sp_BaiViet_ToggleStatus", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
