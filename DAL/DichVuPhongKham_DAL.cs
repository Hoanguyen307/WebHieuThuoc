using Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace DAL
{
    public class DichVuPhongKham_DAL
    {
        public List<DichVuPhongKham> Select_All()
        {
            try
            {
                var result = SqlMapper.Query<DichVuPhongKham>(
                    Connection.getConnection(),
                    "sp_DichVuPhongKham_GetAll",
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DichVuPhongKham SelectById(int id)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@DichVuId", id);
                var model = SqlMapper.Query<DichVuPhongKham>(
                    Connection.getConnection(),
                    "sp_DichVuPhongKham_GetById",  
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                ).FirstOrDefault();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Insert(DichVuPhongKham obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@TenDichVu", obj.TenDichVu);
                param.Add("@Gia", obj.Gia);
                param.Add("@ThoiGian", obj.ThoiGian);
                param.Add("@MoTa", obj.MoTa);
                param.Add("@HinhAnh", obj.HinhAnh);
                param.Add("@CreatedBy", obj.CreatedBy);

                return Connection.getConnection().Execute(
                    "sp_DichVuPhongKham_Insert",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int Update(DichVuPhongKham obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@DichVuId", obj.DichVuId);
                param.Add("@TenDichVu", obj.TenDichVu);
                param.Add("@Gia", obj.Gia);
                param.Add("@ThoiGian", obj.ThoiGian);
                param.Add("@MoTa", obj.MoTa);
                param.Add("@HinhAnh", obj.HinhAnh);
                param.Add("@UpdatedBy", obj.UpdatedBy);

                return Connection.getConnection().Execute(
                    "sp_DichVuPhongKham_Update",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool Delete(int id, string deletedBy)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@DichVuId", id);
                param.Add("@DeletedBy", deletedBy);

                Connection.getConnection().Execute(
                    "sp_DichVuPhongKham_Delete",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
