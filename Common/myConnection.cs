using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using Extension;
using System.Configuration;

namespace Common
{
    public static class Connection
    {

        private static SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DBConnect"].ToString());
        public enum connectionState { open, close };
        public static void setStateConnection(connectionState state)
        {
            if (state == connectionState.open)
            {
                con.Open();
            }
            else
            {
                con.Close();
            }
        }
        public static int Excute(String sql)
        {
            SqlCommand cmd = new SqlCommand(sql, con);
            return cmd.ExecuteScalar().MapInt();
        }
        public static List<T> Select<T>(string StoreProcedueName, DynamicParameters param = null)
          where T : new()
        {
            var result = SqlMapper.Query<T>(Connection.getConnection(), StoreProcedueName, param, commandType: System.Data.CommandType.StoredProcedure).ToList();
            return result;
        }
        public static int Execute(string StoreProcedueName, DynamicParameters param = null)
        {
            return Connection.getConnection().Execute(StoreProcedueName, param, commandType: System.Data.CommandType.StoredProcedure);
        }
        public static IDbConnection getConnection()
        {
            IDbConnection _db = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DBConnect"].ToString());
            return _db;
        }
        public static int ExcuteStoreProc(string StoreProcName, SqlParameter[] param = null)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(StoreProcName);
                cmd.Connection = con;
                if (con.State == ConnectionState.Closed) { con.Open(); }
                cmd.CommandType = CommandType.StoredProcedure;
                if (param != null)
                {
                    for (int i = 0; i < param.Length; i++)
                    {
                        cmd.Parameters.Add(param[i]);
                    }
                }
                return cmd.ExecuteScalar().MapInt();
            }
            catch (Exception)
            {

                throw;
            }

        }
        public static DataTable SelectTable(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dt = ds.Tables[0];
                return dt;
            }
            catch (Exception)
            {
                return dt;
                throw;
            }
            finally
            {
            }
        }
        public static DataTable SelectTableByStoreProc(string StoreProcName, SqlParameter[] param = null)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand(StoreProcName);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                if (param != null)
                {
                    for (int i = 0; i < param.Length; i++)
                    {
                        cmd.Parameters.Add(param[i]);
                    }
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dt = ds.Tables[0];
                return dt;
            }
            catch (Exception)
            {
                return dt;
                throw;
            }
            finally
            {
            }
        }
        public static int GetIDENTITY(String tableName)
        {
            int idx = 0;
            string sql = "Select * FROM " + tableName;
            DataTable dt = SelectTable(sql);
            if (dt.Rows.Count > 0)
            {
                String sql2 = "Select COLUMN_NAME " +
                "FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE " +
                "WHERE TABLE_NAME = '" + tableName + "'";
                String colID = "";
                DataTable dt2 = SelectTable(sql2);
                if (dt2.Rows.Count > 0)
                {
                    colID = dt2.Rows[0][0].ToString();
                }
                if (colID != "")
                {
                    idx = dt.Rows[dt.Rows.Count - 1][colID].ToInt32();
                }
                else
                {
                    idx = dt.Rows[dt.Rows.Count - 1][0].ToInt32();
                }
            }
            return idx;
        }
        #region "SaveLog"
        public enum SuKien
        {
            Insert, Update, Delete, Singin, Singout

        };
        public static int GetIDSuKien(SuKien e)
        {
            switch (e)
            {
                case SuKien.Insert:
                    return 1;
                case SuKien.Update:
                    return 2;
                case SuKien.Delete:
                    return 3;
                case SuKien.Singin:
                    return 4;
                case SuKien.Singout:
                    return 5;
                default:
                    return 0;
            }
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "Not Found";
        }

        public static string GetIpAddress()
        {
            string ipAddress = null;
            try
            {
                HttpRequest currentRequest = HttpContext.Current.Request;
                ipAddress = currentRequest.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress) || (ipAddress.ToLower() == "unknown"))
                {
                    ipAddress = currentRequest.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch (Exception ex)
            {
                ipAddress = "Invalid IP:" + ex.Message;
            }

            return ipAddress;
        }
        public static int SaveLog(SuKien e, String Noi_dung, int UserID)
        {
            try
            {
                SqlParameter[] param = new SqlParameter[9];
                param[0] = new SqlParameter("@ID_phan_he", 1);
                param[1] = new SqlParameter("@Su_kien", GetIDSuKien(e));
                param[2] = new SqlParameter("@HostName", Dns.GetHostName());
                param[3] = new SqlParameter("@Noi_dung", Noi_dung);
                param[4] = new SqlParameter("@Thoi_diem", DateTime.Now);
                param[5] = new SqlParameter("@LocalIPAdress", GetLocalIPAddress());
                param[6] = new SqlParameter("@IPAdress", GetIpAddress());
                param[7] = new SqlParameter("@UserID", UserID);
                param[8] = new SqlParameter("@URL", HttpContext.Current.Request.Url.AbsolutePath);
                return Connection.ExcuteStoreProc("SYS_SuKienNguoiDung_Add", param);
            }
            catch { return 0; }

        }
        #endregion
    }
}
