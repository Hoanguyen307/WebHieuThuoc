using Common;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class FlashSaleProduct_DAL
    {
        public List<FlashSaleProduct> Select_ByFlashSale(int flashSaleId, string keyword = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@FlashSaleId", flashSaleId);
                param.Add("@Keyword", keyword);

                var result = SqlMapper.Query<FlashSaleProduct>(
                    Connection.getConnection(),
                    "sp_FlashSale_GetProducts",
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

        public FlashSaleProduct SelectById(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id);

                var model = SqlMapper.Query<FlashSaleProduct>(
                    Connection.getConnection(),
                    "sp_FlashSaleProduct_GetById",
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

        public int Insert(FlashSaleProduct obj)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@FlashSaleId", obj.FlashSaleId);
                param.Add("@ProductId", obj.ThuocId);
                param.Add("@DiscountPercent", obj.DiscountPercent);

                return Connection.getConnection().Execute(
                    "sp_FlashSale_AddProduct",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int Update(FlashSaleProduct obj)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", obj.Id);
                param.Add("@DiscountPercent", obj.DiscountPercent);
                param.Add("@FlashPrice", obj.FlashPrice);

                return Connection.getConnection().Execute(
                    "sp_FlashSale_UpdateProduct",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool Delete(int id, int productId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id);
                param.Add("@ProductId", productId);

                Connection.getConnection().Execute(
                    "sp_FlashSale_DeleteProduct",
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
