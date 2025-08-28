using Common;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class DiaChiGiaoHang_DAL
    {
        public int Insert(DiaChiGiaoHang obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@KhachHangId", obj.KhachHangId);
                param.Add("@TenNguoiNhan", obj.TenNguoiNhan);
                param.Add("@SoDienThoai", obj.SoDienThoai);
                param.Add("@DiaChiChiTiet", obj.DiaChiChiTiet);
                param.Add("@GhiChu", obj.GhiChu);
                param.Add("@MacDinh", obj.MacDinh);

                return Connection.getConnection().QuerySingle<int>(
                    "sp_DiaChiGiaoHang_Insert",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm địa chỉ giao hàng", ex);
            }
        }

        public bool Update(DiaChiGiaoHang obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@DiaChiId", obj.DiaChiId);
                param.Add("@KhachHangId", obj.KhachHangId);
                param.Add("@TenNguoiNhan", obj.TenNguoiNhan);
                param.Add("@SoDienThoai", obj.SoDienThoai);
                param.Add("@DiaChiChiTiet", obj.DiaChiChiTiet);
                param.Add("@GhiChu", obj.GhiChu);
                param.Add("@MacDinh", obj.MacDinh);

                Connection.getConnection().Execute(
                    "sp_DiaChiGiaoHang_Update",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật địa chỉ giao hàng", ex);
            }
        }

        public bool Delete(int diaChiId, int khachHangId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@DiaChiId", diaChiId);
                param.Add("@KhachHangId", khachHangId);

                Connection.getConnection().Execute(
                    "sp_DiaChiGiaoHang_Delete",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa địa chỉ giao hàng", ex);
            }
        }

        // Lấy danh sách địa chỉ của 1 khách hàng
        public List<DiaChiGiaoHang> GetByKhachHang(int khachHangId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@KhachHangId", khachHangId);

                return Connection.getConnection().Query<DiaChiGiaoHang>(
                    "sp_DiaChiGiaoHang_GetByCustomer",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách địa chỉ giao hàng", ex);
            }
        }

        // Lấy địa chỉ mặc định
        public DiaChiGiaoHang GetDefault(int khachHangId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@KhachHangId", khachHangId);

                return Connection.getConnection().QueryFirstOrDefault<DiaChiGiaoHang>(
                    "sp_LayDiaChiMacDinh",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy địa chỉ mặc định", ex);
            }
        }

        // Đặt địa chỉ mặc định
        public bool SetDefault(int khachHangId, int diaChiId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@KhachHangId", khachHangId);
                param.Add("@DiaChiId", diaChiId);

                Connection.getConnection().Execute(
                    "sp_SetDiaChiMacDinh",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi đặt địa chỉ mặc định", ex);
            }
        }
    }
}
