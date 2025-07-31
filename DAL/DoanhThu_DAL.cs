using Common;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.DoanhThu;

namespace DAL
{
    public class DoanhThu_DAL
    {
        public BaoCaoDoanhThu GetBaoCaoDoanhThu(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@FromDate", fromDate);
                param.Add("@ToDate", toDate);
                /*var result = SqlMapper.Query<BaoCaoDoanhThu>(Connection.getConnection(), "sp_BaoCao_DoanhThu",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();*/
                using (var multi = Connection.getConnection().QueryMultiple("sp_BaoCao_DoanhThu", param, commandType: System.Data.CommandType.StoredProcedure))
                {
                    var tongHop = multi.Read<DoanhThu.DoanhThuTongHop>().FirstOrDefault();

                    var topSanPham = multi.Read<DoanhThu.SanPhamBanChay>().ToList();

                    return new DoanhThu.BaoCaoDoanhThu
                    {
                        TongHop = tongHop,
                        TopSanPham = topSanPham
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
