using Common;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.ProductReview;

namespace DAL
{
    public class ProductReview_DAL
    {
        public List<ProductReview> ReviewGetByProduct(int productId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProductId", productId);

                var result = SqlMapper.Query<ProductReview>(
                    Connection.getConnection(),
                    "sp_ProductReview_ListByProduct",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();

                return result;
            }
            catch (Exception)
            {
                return new List<ProductReview>();
            }
        }
        public List<ProductReview> Select_ProductReview_All(ProductReviewFilter filter)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Search", filter.SearchString);
                param.Add("@Rating", filter.Rating);
                param.Add("@FromDate", filter.FromDate);
                param.Add("@ToDate", filter.ToDate);
                var result = SqlMapper.Query<ProductReview>(Connection.getConnection(), "sp_ProductReview_List",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public int Insert(ProductReview review)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProductId", review.ProductId);
                param.Add("@CustomerId", review.CustomerId);
                param.Add("@Rating", review.Rating);
                param.Add("@Comment", review.Comment);

                return Connection.getConnection().Execute(
                    "sp_ProductReview_Insert",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool DeleteReview(int ID, string TenNguoiXoa)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", ID);
                param.Add("@DeletedBy", TenNguoiXoa);
                Connection.getConnection().Execute("sp_ProductReview_Delete", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public decimal GetAverageRating(int productId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProductId", productId);

                return SqlMapper.QueryFirstOrDefault<decimal>(
                    Connection.getConnection(),
                    "sp_ProductReview_AverageRating",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
