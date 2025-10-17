using Common;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Chat_DAL
    {
        public int CreateSession(int? customerId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CustomerId", customerId);

                var sessionId = SqlMapper.Query<int>(
                    Connection.getConnection(),
                    "sp_ChatSession_Create",
                    param,
                    commandType: CommandType.StoredProcedure
                ).FirstOrDefault();

                return sessionId;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public ChatSession GetSessionByCustomer(int customerId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@CustomerId", customerId);

                var session = SqlMapper.Query<ChatSession>(
                    Connection.getConnection(),
                    "sp_ChatSession_GetByCustomer",
                    param,
                    commandType: CommandType.StoredProcedure
                ).FirstOrDefault();

                return session;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int InsertMessage(ChatMessage obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@SessionId", obj.SessionId);
                param.Add("@SenderType", obj.SenderType);
                param.Add("@SenderId", obj.SenderId);
                param.Add("@Message", obj.Message);

                return Connection.getConnection().Execute(
                    "sp_ChatMessage_Insert",
                    param,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public List<ChatMessage> GetMessagesBySession(int sessionId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@SessionId", sessionId);

                var result = SqlMapper.Query<ChatMessage>(
                    Connection.getConnection(),
                    "sp_ChatMessage_GetBySession",
                    param,
                    commandType: CommandType.StoredProcedure
                ).ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ChatSession> GetAllSessions()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                var result = SqlMapper.Query<ChatSession>(
                    Connection.getConnection(),
                    "sp_ChatMessage_GetAll",
                    param,
                    commandType: CommandType.StoredProcedure
                ).ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Product> ProductsByKeywords(List<string> keywords)
        {
            try
            {
                if (keywords == null || !keywords.Any())
                {
                    return new List<Product>();
                }

                // 1. Tạo một DataTable để chứa danh sách từ khóa.
                // Tên cột "Keyword" phải khớp với tên cột trong TYPE bạn đã tạo trong SQL.
                var keywordsTable = new DataTable();
                keywordsTable.Columns.Add("Keyword", typeof(string));
                foreach (var keyword in keywords)
                {
                    keywordsTable.Rows.Add(keyword);
                }

                var param = new DynamicParameters();

                param.Add("@Keywords", keywordsTable.AsTableValuedParameter("dbo.KeywordList"));

                var result = SqlMapper.Query<Product>(
                    Connection.getConnection(),
                    "sp_Products_ByKeyword",
                    param,
                    commandType: CommandType.StoredProcedure
                ).ToList();

                return result;
            }
            catch (Exception ex)
            {
                // Log lỗi ra để dễ dàng debug
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
