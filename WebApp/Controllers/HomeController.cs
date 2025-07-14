using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private DBConnect db = new DBConnect();
        public ActionResult Index(int? processId)
        {
            if (processId.HasValue)
            {
                ViewBag.ProcessId = processId.Value; 
            }

            return View();
        }

        public ActionResult Add(int? id)
        {
            var tientrinh = new Process();
            var listUser = new Account_DAL().Select_NguoiDung_All();
            ViewBag.Users = new SelectList(listUser, "Id", "UserName");

            var listBuilding = new Process_DAL().Select_ToaNha_All();
            ViewBag.Buildings = new SelectList(listBuilding, "BuildingId", "BuildingName");

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
                model.TotalSteps = 7;

                var newProcessId = new Process_DAL().Insert(model);
                if (newProcessId > 0)
                {
                    return Json(new
                    {
                        processId = newProcessId,
                        currentStep = 1,
                        lastCompletedStep = 0,
                        code = 200,
                        msg = "Thêm mới thành công"
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Thêm mới thất bại" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateProcessStep(Process model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Dữ liệu yêu cầu không hợp lệ." });
            }
            try
            {
                var existing = Process_DAL.GetProcessState(model.ProcessId); 
                if (existing == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy tiến trình." });
                }
                int updatedLastCompletedStep = model.LastCompletedStep > existing.LastCompletedStep
                    ? model.LastCompletedStep
                    : existing.LastCompletedStep;
                int totalSteps = 7;
                var processToUpdate = new Process
                {
                    ProcessId = model.ProcessId,
                    CurrentStep = model.CurrentStep,
                    LastCompletedStep = updatedLastCompletedStep,
                    LastUpdatedDate = DateTime.Now
                };

                bool success = Process_DAL.Update(processToUpdate, totalSteps);
                if (success)
                {
                    return Json(new { success = true, message = "Cập nhật bước tiến trình thành công." });
                }
                else
                {
                    return Json(new { success = false, message = "Cập nhật bước tiến trình thất bại." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi server: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetProcessState(int processId)
        {
            try
            {
                var process = Process_DAL.GetProcessState(processId);

                if (process != null)
                {
                    return Json(new { success = true, processId = processId, currentStep = process.CurrentStep, lastCompletedStep = process.LastCompletedStep }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy tiến trình này.", processId = 0, currentStep = 1, lastCompletedStep = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi tải trạng thái tiến trình: " + ex.Message, currentStep = 1, lastCompletedStep = 0 }, JsonRequestBehavior.AllowGet);
            }
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
        [HttpPost]
        public JsonResult Delete(int Id)
        {
            try
            {
                var result = new Process_DAL().Delete(Id);
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
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}