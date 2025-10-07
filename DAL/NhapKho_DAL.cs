using Common;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Post;

namespace DAL
{
    public class NhapKho_DAL
    {
        private DataTable CreateChiTietNhapKhoDataTable(IEnumerable<ChiTietNhapKho> chiTietList)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("ProductId", typeof(int));
            dataTable.Columns.Add("SoLuong", typeof(int));
            dataTable.Columns.Add("DonGiaNhap", typeof(decimal));
            dataTable.Columns.Add("NgaySanXuat", typeof(DateTime));
            dataTable.Columns.Add("HanSuDung", typeof(int));

            if (chiTietList != null)
            {
                foreach (var item in chiTietList)
                {
                    dataTable.Rows.Add(item.ProductId, item.SoLuong, item.DonGiaNhap, item.NgaySanXuat, item.HanSuDung);
                }
            }

            return dataTable;
        }
        public List<NhapKho> Select_NhapKho_All(NhapKhoFilter filter)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@MaPhieu", filter.MaPhieu);
                param.Add("@Month", filter.Month);
                param.Add("@Year", filter.Year);
                var result = SqlMapper.Query<NhapKho>(Connection.getConnection(), "sp_NhapKho_GetAll",
                    param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public NhapKho SelectById(int NhapKhoId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@NhapKhoId", NhapKhoId);

                using (var multi = Connection.getConnection().QueryMultiple("sp_NhapKho_GetById",
                    param, commandType: System.Data.CommandType.StoredProcedure))
                {
                    var nhapKho = multi.Read<NhapKho>().FirstOrDefault();
                    if (nhapKho != null)
                    {
                        nhapKho.ChiTietNhapKho = multi.Read<ChiTietNhapKho>().ToList();
                    }
                    return nhapKho;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Insert(NhapKho obj)
        {
            try
            {
                using (var connection = Connection.getConnection())
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@MaPhieu", obj.MaPhieu);
                    param.Add("@NgayNhap", obj.NgayNhap);
                    param.Add("@NguoiNhap", obj.NguoiNhap);
                    param.Add("@NhaCungCap", obj.NhaCungCap);
                    param.Add("@GhiChu", obj.GhiChu);
                    param.Add("@TotalAmount", obj.TotalAmount);
                    param.Add("@CreatedBy", obj.CreatedBy);

                    if (obj.ChiTietNhapKho != null && obj.ChiTietNhapKho.Any())
                    {
                        var chiTietTable = CreateChiTietNhapKhoDataTable(obj.ChiTietNhapKho);
                        param.Add("@ChiTietNhapKho", chiTietTable.AsTableValuedParameter("dbo.NhapKhoChiTietType"));
                    }

                    return connection.Execute("sp_NhapKho_Insert", param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Update(NhapKho obj)
        {
            try
            {
                using (var connection = Connection.getConnection())
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@NhapKhoId", obj.Id);
                    param.Add("@MaPhieu", obj.MaPhieu);
                    param.Add("@NguoiNhap", obj.NguoiNhap);
                    param.Add("@NhaCungCap", obj.NhaCungCap);
                    param.Add("@GhiChu", obj.GhiChu);
                    param.Add("@UpdatedBy", obj.UpdatedBy);
                    param.Add("@TotalAmount", obj.TotalAmount);

                    var chiTietDataTable = CreateChiTietNhapKhoDataTable(obj.ChiTietNhapKho);

                    param.Add("@ChiTietNhapKho", chiTietDataTable.AsTableValuedParameter("dbo.NhapKhoChiTietType"));

                    return connection.Execute("sp_NhapKho_Update", param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Delete(int Id, string DeletedBy)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@NhapKhoId", Id);
                param.Add("@DeletedBy", DeletedBy);

                Connection.getConnection().Execute("sp_NhapKho_Delete", param, commandType: System.Data.CommandType.StoredProcedure);
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
