using Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using static Models.FlashSale;

namespace DAL
{
    public class FlashSale_DAL
    {
        public List<FlashSale> Select_All()
        {
            try
            {
                var result = SqlMapper.Query<FlashSale>(
                    Connection.getConnection(),
                    "sp_FlashSales_GetAll",
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public FlashSale SelectById(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@FlashSaleId", id);

                var model = SqlMapper.Query<FlashSale>(
                    Connection.getConnection(),
                    "sp_FlashSale_GetById",
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

        public int Insert(FlashSale obj)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Title", obj.Title);
                param.Add("@StartTime", obj.StartTime);
                param.Add("@EndTime", obj.EndTime);
                param.Add("@IsActive", obj.IsActive);
                param.Add("@CreatedBy", obj.CreatedBy);
                param.Add("@DiscountPercent", obj.DiscountPercent);
                param.Add("@DiscountAmount", obj.DiscountAmount);

                return Connection.getConnection().Execute(
                    "sp_FlashSale_Insert",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int Update(FlashSale obj)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", obj.Id);
                param.Add("@Title", obj.Title);
                param.Add("@StartTime", obj.StartTime);
                param.Add("@EndTime", obj.EndTime);
                param.Add("@IsActive", obj.IsActive);
                param.Add("@DiscountPercent", obj.DiscountPercent);
                param.Add("@DiscountAmount", obj.DiscountAmount);

                return Connection.getConnection().Execute(
                    "sp_FlashSale_Update",
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
                var param = new DynamicParameters();
                param.Add("@Id", id);
                param.Add("@DeletedBy", deletedBy);

                Connection.getConnection().Execute(
                    "sp_FlashSale_Delete",
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


        public bool Update_IsActive(int Id, bool isActive)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", Id);
                param.Add("@IsActive", isActive);
                Connection.getConnection().Execute("sp_FlashSales_Update_IsActive", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<FlashSale> Select_ActiveFlashSale()
        {
            try
            {
                var param = new DynamicParameters();

                var model = SqlMapper.Query<FlashSale>(
                    Connection.getConnection(),
                    "sp_Select_ActiveFlashSale",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
