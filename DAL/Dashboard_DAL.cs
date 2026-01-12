using Common;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Dashboard_DAL
    {

        public DuLieuBanHang LayDuLieuBanHang(string kyHan, DateTime? tuNgay, DateTime? denNgay)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@KyHan", kyHan);
                param.Add("@TuNgay", tuNgay); 
                param.Add("@DenNgay", denNgay);

                var result = SqlMapper.Query(Connection.getConnection(),
                    "Dashboard_LayDuLieuBanHang", param,
                    commandType: System.Data.CommandType.StoredProcedure).ToList();

                var duLieuBanHang = new DuLieuBanHang
                {
                    NhanThoiGian = new List<string>(),
                    BanHang = new List<decimal?>(),
                    LoiNhuan = new List<decimal?>()
                };

                foreach (dynamic item in result)
                {
                    if (item.NhanThoiGian is DateTime)
                    {
                        duLieuBanHang.NhanThoiGian.Add(((DateTime)item.NhanThoiGian).ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        duLieuBanHang.NhanThoiGian.Add(item.NhanThoiGian.ToString());
                    }
                    duLieuBanHang.BanHang.Add(item.BanHang);
                    duLieuBanHang.LoiNhuan.Add(item.LoiNhuan);
                }

                return duLieuBanHang;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ThongKeTongQuan LayThongKeTongQuan(DateTime? tuNgay, DateTime? denNgay)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@TuNgay", tuNgay); // Truyền tham số lọc
            param.Add("@DenNgay", denNgay);

            return SqlMapper.Query<ThongKeTongQuan>(Connection.getConnection(),
                "Dashboard_LayThongKeTongQuan", param,
                commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
        }

        public TongKetTaiChinh LayTongKetTaiChinh(DateTime? tuNgay, DateTime? denNgay)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@TuNgay", tuNgay);
            param.Add("@DenNgay", denNgay);

            return SqlMapper.Query<TongKetTaiChinh>(Connection.getConnection(),
                "Dashboard_LayTongKetTaiChinh", param,
                commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
        }
    }
}
