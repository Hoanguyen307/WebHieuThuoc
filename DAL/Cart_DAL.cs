using Common;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Cart_DAL
    {
        public int AddToCart(int userId, int productId, int quantity)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@UserId", userId);
                param.Add("@ProductId", productId);
                param.Add("@Quantity", quantity);

                return Connection.getConnection().Execute(
                    "sp_AddToCart",
                    param,
                    commandType: System.Data.CommandType.StoredProcedure
                );
            }
            catch (Exception)
            {
                throw new Exception("An error occurred while adding the item to the cart. Please try again later.");
            }
        }

        public List<CartItemModel> GetCartByCustomer(int customerId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CustomerId", customerId);
                var result = SqlMapper.Query<CartItemModel>(
                    Connection.getConnection(),
                    "sp_GetCartByCustomer",
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

        public bool UpdateQuantity(int cartItemId, int quantity)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CartItemId", cartItemId);
                param.Add("@Quantity", quantity);

                Connection.getConnection().Execute(
                    "sp_UpdateCartItemQuantity",
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

        public bool RemoveItem(int cartItemId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CartItemId", cartItemId);

                Connection.getConnection().Execute(
                    "sp_RemoveCartItem",
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

        public bool ClearCart(int customerId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CustomerId", customerId);

                Connection.getConnection().Execute(
                    "sp_ClearCart",
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
