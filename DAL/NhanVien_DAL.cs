using Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using static Models.NhanVien;
using static Models.LichLamViec;
using System.Security.Cryptography;
using System.Data;
using Models.LichLamViecViewModel;

namespace DAL
{
    public class NhanVien_DAL
    {
        public List<CaLam> Select_CaLam_All()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var result = SqlMapper.Query<CaLam>(Connection.getConnection(), "sp_CaLam_GetAll",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<Position> Select_Position_All()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var result = SqlMapper.Query<Position>(Connection.getConnection(), "sp_Position_GetAll",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<NhanVien> Select_NhanVien_All(NhanVienFilter filter)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@FullName", filter.FullName);
                param.Add("@Month", filter.Month);
                param.Add("@Year", filter.Year);
                param.Add("@PositionId", filter.PositionId);
                param.Add("@ShiftId", filter.ShiftId);
                var result = SqlMapper.Query<NhanVien>(Connection.getConnection(), "sp_Employee_GetAll",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<LichLamViec> LichLamViec(LichLamViecFilter filter)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@FromDate", filter.FromDate);
                param.Add("@ToDate", filter.ToDate);
                param.Add("@PositionId", filter.PositionId);
                param.Add("@ShiftId", filter.ShiftId);
                var result = SqlMapper.Query<LichLamViec>(Connection.getConnection(), "sp_LichLamViec",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<LichSu_ChucVu> Select_LichSu_All()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var result = SqlMapper.Query<LichSu_ChucVu>(Connection.getConnection(), "sp_LichSuChucVu_GetAll",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<LichSu_ChucVu> LichSu_SelectById(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@NhanVienId", ID);
                var model = SqlMapper.Query<LichSu_ChucVu>(Connection.getConnection(), "sp_LichSuChucVu_Get", param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public NhanVien SelectById(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", ID);
                var model = SqlMapper.Query<NhanVien>(Connection.getConnection(), "sp_Employee_GetById", param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Insert(NhanVien obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@FullName", obj.FullName);
                param.Add("@Gender", obj.Gender);
                param.Add("@BirthDate", obj.BirthDate);
                param.Add("@Phone", obj.Phone);
                param.Add("@Email", obj.Email);
                param.Add("@PositionId", obj.PositionId);
                param.Add("@Salary", obj.Salary);
                param.Add("@StartDate", obj.StartDate);
                param.Add("@ShiftId", obj.ShiftId);
                param.Add("@UsersId", obj.UsersId);
                param.Add("@CreatedBy", obj.CreatedBy);
                return Connection.getConnection().Execute("sp_Employee_Add", param, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }
        public int XepLich(int nhanVienId, string createdBy, List<LichTrongTuanModel> lichTrongTuanList)
        {
            try
            {
                var table = new DataTable();
                table.Columns.Add("NgayLam", typeof(DateTime));
                table.Columns.Add("ShiftId", typeof(int));

                foreach (var item in lichTrongTuanList)
                {
                    table.Rows.Add(item.NgayLam, item.ShiftId);
                }

                DynamicParameters param = new DynamicParameters();
                param.Add("@NhanVienId", nhanVienId);
                param.Add("@CreatedBy", createdBy);
                param.Add("@LichTrongTuan", table.AsTableValuedParameter("LichTrongTuanType"));

                return Connection.getConnection().Execute("sp_XepLichTheoTuan", param, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int Update(NhanVien obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", obj.Id);
                param.Add("@FullName", obj.FullName);
                param.Add("@Gender", obj.Gender);
                param.Add("@BirthDate", obj.BirthDate);
                param.Add("@Phone", obj.Phone);
                param.Add("@Email", obj.Email);
                param.Add("@PositionId", obj.PositionId);
                param.Add("@Salary", obj.Salary);
                param.Add("@StartDate", obj.StartDate);
                param.Add("@ShiftId", obj.ShiftId);
                param.Add("@UsersId", obj.UsersId);
                param.Add("@UpdatedBy", obj.UpdatedBy);
                return Connection.getConnection().Execute("sp_Employee_Update", param, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }
        public int UpdateTime(LichLamViec obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", obj.Id);
                param.Add("@UpdatedBy", obj.UpdatedBy);
                param.Add("@NewThoiGian", obj.NewThoiGian);
                return Connection.getConnection().Execute("sp_LichLamViec_Update", param, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }
        public int UpdateCaLam(LichLamViec obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", obj.Id);
                param.Add("@ShiftId", obj.ShiftId);
                param.Add("@UpdatedBy", obj.UpdatedBy);
                return Connection.getConnection().Execute("sp_LichLamViec_CaLam", param, commandType: System.Data.CommandType.StoredProcedure);
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
                Connection.getConnection().Execute("sp_Employee_Delete", param, commandType: System.Data.CommandType.StoredProcedure);
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
