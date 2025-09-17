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
        public ThongKeTongQuan LayThongKeTongQuan()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var result = SqlMapper.Query<ThongKeTongQuan>(Connection.getConnection(),
                    "Dashboard_LayThongKeTongQuan", param,
                    commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DuLieuBanHang LayDuLieuBanHang(string kyHan)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@KyHan", kyHan);

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
                        // Nếu là chuỗi 'Tuần 36' hoặc 'Tháng 9'
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

        public TongKetTaiChinh LayTongKetTaiChinh()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var result = SqlMapper.Query<TongKetTaiChinh>(Connection.getConnection(),
                    "Dashboard_LayTongKetTaiChinh", param,
                    commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
