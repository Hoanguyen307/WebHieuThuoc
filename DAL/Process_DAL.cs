using Common;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Process;

namespace DAL
{
    public class Process_DAL
    {
        public List<Process> Select_Process_All(ProcessFilter filter)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@AccountId", filter.AccountId);
                param.Add("@BuildingId", filter.BuildingId);
                param.Add("@Month", filter.Month);
                param.Add("@Year", filter.Year);
                var result = SqlMapper.Query<Process>(Connection.getConnection(), "SP_GetProcessList",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<Buidings> Select_ToaNha_All()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var result = SqlMapper.Query<Buidings>(Connection.getConnection(), "sp_ToaNha_GetAll",
               param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public int Insert(Process obj)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ProcessName", obj.ProcessName);
                param.Add("@BuildingId", obj.BuildingId);
                param.Add("@TotalSteps", obj.TotalSteps);
                param.Add("@AccountId", obj.AccountId);
                param.Add("@CreatedBy", obj.CreatedBy);
                int newProcessId = Connection.getConnection().QueryFirstOrDefault<int>("SP_CreateProcess", param, commandType: System.Data.CommandType.StoredProcedure);

                return newProcessId;
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }
        public static bool Update(Process obj, int totalSteps)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ProcessId", obj.ProcessId);
                param.Add("@CurrentStep", obj.CurrentStep);
                param.Add("@LastCompletedStep", obj.LastCompletedStep);
                param.Add("@TotalSteps", totalSteps);
                Connection.getConnection().Execute("UpdateProcessSteps", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public bool Delete(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ProcessId", ID);
                Connection.getConnection().Execute("sp_Process_Delete", param, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public static Process GetProcessState(int processId)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ProcessId", processId);
                return Connection.getConnection().Query<Process>("GetProcessState", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy trạng thái tiến trình (ID: {processId}): {ex.Message}");
                return null; 
            }
        }
        public static int? GetLatestProcessId()
        {
            using (var db = new DBConnect())
            {
                var latest = db.Processes
                               .AsNoTracking() // ✅ Không theo dõi, tránh lazy loading User
                               .Where(p => !p.IsDeleted)
                               .OrderByDescending(p => p.ProcessId)
                               .Select(p => p.ProcessId) // ✅ Chỉ lấy ProcessId
                               .FirstOrDefault();

                return latest;
            }
        }

    }
}
