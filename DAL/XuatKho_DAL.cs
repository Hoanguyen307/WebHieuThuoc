using Common;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.XuatKho;

namespace DAL
{
    public class XuatKho_DAL
    {
        private DataTable CreateChiTietXuatKhoDataTable(IEnumerable<ChiTietXuatKho> chiTietList)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("ProductId", typeof(int));
            dataTable.Columns.Add("SoLuong", typeof(int));
            dataTable.Columns.Add("DonGiaXuat", typeof(decimal));

            if (chiTietList != null)
            {
                foreach (var item in chiTietList)
                {
                    dataTable.Rows.Add(item.ProductId, item.SoLuong, item.DonGiaXuat);
                }
            }

            return dataTable;
        }
        public List<XuatKho> Select_XuatKho_All(XuatKhoFilter filter)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@MaPhieu", filter.MaPhieu);
                param.Add("@Month", filter.Month);
                param.Add("@Year", filter.Year);
                var result = SqlMapper.Query<XuatKho>(Connection.getConnection(), "sp_XuatKho_GetAll",
                    param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public XuatKho SelectById(int XuatKhoId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@XuatKhoId", XuatKhoId);

                using (var multi = Connection.getConnection().QueryMultiple("sp_XuatKho_GetById",
                    param, commandType: System.Data.CommandType.StoredProcedure))
                {
                    var xuatKho = multi.Read<XuatKho>().FirstOrDefault();
                    if (xuatKho != null)
                    {
                        xuatKho.ChiTietXuatKho = multi.Read<ChiTietXuatKho>().ToList();
                    }
                    return xuatKho;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
