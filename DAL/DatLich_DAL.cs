using Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace DAL
{
    public class DatLich_DAL
    {
        public int Insert(DatLichPhongKham obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@KhachHangId", obj.KhachHangId);
                param.Add("@DichVuId", obj.DichVuId);
                param.Add("@NgayDat", obj.NgayDat);
                param.Add("@KhungGio", obj.KhungGio);
                param.Add("@CreatedBy", obj.CreatedBy);

                return Connection.getConnection().Execute(
                    "sp_DatLichPhongKham",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int UpdateTrangThai(int datLichId, string trangThai, int? bacSiId, string updatedBy)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@DatLichId", datLichId);
                param.Add("@TrangThai", trangThai);
                param.Add("@BacSiId", bacSiId);
                param.Add("@UpdatedBy", updatedBy);

                return Connection.getConnection().Execute(
                    "sp_TrangThaiDatLich_Update",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public List<DatLichPhongKham> GetByKhachHang(int khachHangId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@KhachHangId", khachHangId);

                var result = SqlMapper.Query<DatLichPhongKham>(
                    Connection.getConnection(),
                    "sp_LayLichHenTheoKhachHang",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DatLichPhongKham> GetAll()
        {
            try
            {
                var result = SqlMapper.Query<DatLichPhongKham>(
                    Connection.getConnection(),
                    "sp_LichHen_GetAll",
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
