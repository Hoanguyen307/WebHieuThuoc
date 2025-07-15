using DAL;
using Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Models.Process;

namespace WebApp.Controllers
{
    public class CaseController : Controller
    {
        private DBConnect db = new DBConnect();
        // GET: Case
        public ActionResult Index(string searchString, int? AccountId, int? BuildingId, int? Month, int? Year)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var year = Enumerable.Range(currentYear - 5, 11).Select(y => new { Id = y, Name = y.ToString() }).ToList();
            ViewBag.Years = new SelectList(year, "Id", "Name", Year);

            var months = Enumerable.Range(1, 12).Select(m => new { Id = m, Name = $"Tháng {m}" }).ToList();
            ViewBag.Months = new SelectList(months, "Id", "Name", Month);

            var listBuilding = new Process_DAL().Select_ToaNha_All();
            ViewBag.Buildings = new SelectList(listBuilding, "BuildingId", "BuildingName", BuildingId);

            if (Month == 0) Month = null;
            if (Year == 0) Year = null;

            ProcessFilter filter = new ProcessFilter
            {
                ProcessName = searchString, 
                AccountId = AccountId,
                BuildingId = BuildingId,
                Month = Month,
                Year = Year
            };

            List<Process> tt = new Process_DAL().Select_Process_All(filter);

            return View();
        }
        public ActionResult GetDropdownData()
        {
            try
            {
                //var reponse = await _httpClient.GetAsync("api/services/app/TnYeuCaus/GetAllTnToaNhaForLookupTable");
                //reponse.EnsureSuccessStatusCode();

                //var jsonString = await reponse.Content.ReadAsStringAsync();
                //var toaNhaResult = JsonConvert.DeserializeObject<LookupResult>(jsonString);

                var currentYear = DateTime.Now.Year;
                var currentMonth = DateTime.Now.Month;
                var year = Enumerable.Range(currentYear - 5, 11).Select(y => new { Id = y, Name = y.ToString() }).ToList();

                var months = Enumerable.Range(1, 12).Select(m => new { Id = m, Name = $"Tháng {m}" }).ToList();

                var listBuilding = new Process_DAL().Select_ToaNha_All();

                return Json(new
                {
                    success = true,
                    toaNha = listBuilding,
                    months = months,
                    year = year,
                    currentYear = currentYear,
                    currentMonth = currentMonth
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Add(int? id)
        {
            var tientrinh = new Process();
            var listUser = new Account_DAL().Select_NguoiDung_All();
            ViewBag.Users = new SelectList(listUser, "Id", "UserName");
            if (id != null)
            {
                var bv = db.Processes.Find(id);
                return PartialView("Add", bv);
            }
            return PartialView("Add", tientrinh);
        }

        [HttpPost]
        public JsonResult Add(Process model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { code = 400, msg = "Dữ liệu không hợp lệ." }, JsonRequestBehavior.AllowGet);
                }
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = Session["UserName"] != null ? Session["UserName"].ToString() : "Unknown";
                model.IsDeleted = false;

                var newProcessId = new Process_DAL().Insert(model);
                if (newProcessId > 0)
                {
                    return Json(new { id = newProcessId, code = 200, msg = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Thêm mới thất bại" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Delete(int ID)
        {
            try
            {
                var result = new Process_DAL().Delete(ID);
                if (result)
                {
                    return Json(new { code = 200, msg = "Xóa thành công" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Xóa thất bại" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetProcesses(string searchString, int? BuildingId, int? Month, int? Year, int page = 1, int pageSize = 10)
        {
            if (Month == 0) Month = null;
            if (Year == 0) Year = null;

            ProcessFilter filter = new ProcessFilter
            {
                ProcessName = searchString,
                BuildingId = BuildingId,
                Month = Month,
                Year = Year
            };

            var processes = new Process_DAL().Select_Process_All(filter);
            var pagedList = processes.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);

            return Json(new
            {
                items = pagedList.ToList(),
                totalCount = pagedList.TotalItemCount,
                currentPage = pagedList.PageNumber,
                pageSize = pagedList.PageSize
            }, JsonRequestBehavior.AllowGet);
        }
    }
}