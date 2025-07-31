using DAL;
using Models;
using Models.LichLamViecViewModel;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Models.KhachHang;
using static Models.LichLamViec;
using static Models.NhanVien;

namespace Admin.Controllers
{
    public class NhanVienController : Controller
    {
        // GET: Admin/NhanVien
        private DBConnect db = new DBConnect();

        public ActionResult Index(string searchString, int? Month, int? Year, int? PositionId, int? ShiftId)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var year = Enumerable.Range(currentYear - 5, 11).Select(y => new { Id = y, Name = y.ToString() }).ToList();
            ViewBag.Years = new SelectList(year, "Id", "Name", Year);

            var months = Enumerable.Range(1, 12).Select(m => new { Id = m, Name = $"Tháng {m}" }).ToList();
            ViewBag.Months = new SelectList(months, "Id", "Name", Month);

            var listPosition = new NhanVien_DAL().Select_Position_All();
            ViewBag.Positions = new SelectList(listPosition, "Id", "Name", PositionId);

            var listCaLam = new NhanVien_DAL().Select_CaLam_All();
            ViewBag.Calams = new SelectList(listCaLam, "Id", "Name", ShiftId);

            return View();
        }

        [HttpGet]
        public JsonResult GetNhanVien(string searchString, int? Month, int? Year, int? PositionId, int? ShiftId, int page = 1, int pageSize = 10)
        {
            if (Month == 0) Month = null;
            if (Year == 0) Year = null;

            NhanVienFilter filter = new NhanVienFilter
            {
                FullName = string.IsNullOrWhiteSpace(searchString) ? null : searchString,
                Month = Month,
                Year = Year,
                PositionId = PositionId,
                ShiftId = ShiftId,
            };

            var processes = new NhanVien_DAL().Select_NhanVien_All(filter);
            var pagedList = processes.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);

            return Json(new
            {
                items = pagedList.ToList(),
                totalCount = pagedList.TotalItemCount,
                currentPage = pagedList.PageNumber,
                pageSize = pagedList.PageSize
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index1(string searchString, DateTime? FromDate, DateTime? ToDate, int? PositionId, int? ShiftId)
        {
            var listPosition = new NhanVien_DAL().Select_Position_All();
            ViewBag.Positions = new SelectList(listPosition, "Id", "Name", PositionId);

            var listCaLam = new NhanVien_DAL().Select_CaLam_All();
            ViewBag.Calams = new SelectList(listCaLam, "Id", "Name", ShiftId);

            return View();
        }
        [HttpGet]
        public JsonResult LichLamViec(string searchString, DateTime? FromDate, DateTime? ToDate, int? PositionId, int? ShiftId, int page = 1, int pageSize = 10)
        {

            LichLamViecFilter filter = new LichLamViecFilter
            {
                FromDate = FromDate,
                ToDate = ToDate,
                PositionId = PositionId,
                ShiftId = ShiftId,
            };
            try
            {
                var processes = new NhanVien_DAL().LichLamViec(filter);
                var pagedList = processes.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);
                return Json(new
                {
                    result = "success",
                    data = pagedList.ToList(),
                    totalCount = pagedList.TotalItemCount,
                    currentPage = pagedList.PageNumber,
                    pageSize = pagedList.PageSize
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    result = "error",
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateTime(LichLamViec model)
        {
            try
            {
                model.UpdatedBy = Session["UserName"] != null ? Session["UserName"].ToString() : "admin";
                model.UpdatedDate = DateTime.Now;
                model.IsDeleted = false;

                var result = new NhanVien_DAL().UpdateTime(model);
                if (result > 0)
                {
                    return Json(new { code = 200, msg = "Cập nhật thành công", icon = "success" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Cập nhật thất bại", icon = "warning" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message, icon = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateCalam(LichLamViec model)
        {
            try
            {
                var listCaLam = new NhanVien_DAL().Select_CaLam_All();
                ViewBag.Calams = new SelectList(listCaLam, "Id", "Name", model.ShiftId);

                model.UpdatedBy = Session["UserName"] != null ? Session["UserName"].ToString() : "admin";
                model.UpdatedDate = DateTime.Now;
                model.IsDeleted = false;

                var result = new NhanVien_DAL().UpdateCaLam(model);
                if (result > 0)
                {
                    return Json(new { code = 200, msg = "Cập nhật thành công", icon = "success" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Cập nhật thất bại", icon = "warning" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message, icon = "error" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult LichSuChucVu(int id)
        {
            try
            {
                List<LichSu_ChucVu> lstmodel = new NhanVien_DAL().LichSu_SelectById(id);
                if (lstmodel != null && lstmodel.Any())
                {
                    var data = lstmodel.Select(x => new
                    {
                        x.PositionName,
                        TuNgay = x.TuNgay?.ToString("yyyy-MM-dd"),
                        DenNgay = x.DenNgay?.ToString("yyyy-MM-dd")
                    }).ToList();

                    return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = true, data = new List<object>() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ChiTietNhanVien(int id)
        {
            try
            {
                var nhanvien = new NhanVien_DAL().SelectById(id);
                if (nhanvien != null)
                {
                    return Json(new { success = true, data = nhanvien }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Không tìm thấy nhân viên" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Add(int? id)
        {
            var nhanvien = new NhanVien();
            var listCaLam = new NhanVien_DAL().Select_CaLam_All();
            ViewBag.Calams = new SelectList(listCaLam, "Id", "Name");

            var listPosition = new NhanVien_DAL().Select_Position_All();
            ViewBag.Positions = new SelectList(listPosition, "Id", "Name");

            var listUser = new Account_DAL().Select_NguoiDung_All();
            ViewBag.Users = new SelectList(listUser, "Id", "UserName");

            if (id != null)
            {
                var nv = db.NhanViens.Find(id);
                return PartialView("Add", nv);
            }
            return PartialView("Add", nhanvien);
        }

        [HttpPost]
        public JsonResult Add(NhanVien model)
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

                var result = new NhanVien_DAL().Insert(model);
                if (result > 0)
                {
                    return Json(new { id = result, code = 200, msg = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Thêm mới thất bại" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                return HttpNotFound();
            }
            NhanVien lstmodel = new NhanVien_DAL().SelectById(id);
            if (lstmodel == null)
            {
                return HttpNotFound();
            }
            var listCaLam = new NhanVien_DAL().Select_CaLam_All();
            ViewBag.Calams = new SelectList(listCaLam, "Id", "Name");

            var listPosition = new NhanVien_DAL().Select_Position_All();
            ViewBag.Positions = new SelectList(listPosition, "Id", "Name");

            var listUser = new Account_DAL().Select_NguoiDung_All();
            ViewBag.Users = new SelectList(listUser, "Id", "UserName");

            return PartialView("Add", lstmodel);
        }
        [HttpPost]
        public JsonResult Update(NhanVien model)
        {
            try
            {
                model.UpdatedBy = Session["UserName"] != null ? Session["UserName"].ToString() : "Unknown";
                model.UpdatedDate = DateTime.Now;
                model.IsDeleted = false;

                var result = new NhanVien_DAL().Update(model);
                if (result > 0)
                {
                    return Json(new { code = 200, msg = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { code = 500, msg = "Cập nhật thất bại" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult XepLich(int? id)
        {
            var nhanvien = new NhanVien();

            var listCaLam = new NhanVien_DAL().Select_CaLam_All();
            ViewBag.Calams = listCaLam;

            if (id != null)
            {
                var nv = db.LichLamViecs.Find(id);
                return PartialView("XepLich", nv);
            }
            return PartialView("XepLich", nhanvien);
        }

        [HttpPost]
        public JsonResult XepLich(int nhanVienId, List<LichTrongTuanModel> lichTrongTuan)
        {
            try
            {
                if (lichTrongTuan == null || !lichTrongTuan.Any())
                {
                    return Json(new { code = 400, msg = "Chưa chọn lịch làm việc." });
                }

                string createdBy = Session["UserName"] != null ? Session["UserName"].ToString() : "Unknown";

                var result = new NhanVien_DAL().XepLich(nhanVienId, createdBy, lichTrongTuan);
                if (result > 0)
                {
                    return Json(new { code = 200, msg = "Xếp lịch thành công." });
                }

                return Json(new { code = 500, msg = "Xếp lịch thất bại." });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message });
            }
        }


        [HttpPost]
        public JsonResult DeleteAccount(int Id, string TenNguoiXoa)
        {
            try
            {
                var result = new NhanVien_DAL().Delete(Id, TenNguoiXoa);
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
        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = db.Products.Find(Convert.ToInt32(item));
                        db.Products.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult IsActive(int ID)
        {
            var item = db.Products.Find(ID);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isActive = item.IsActive });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult ImportExcel(HttpPostedFileBase excelFile)
        {
            if (excelFile == null || excelFile.ContentLength == 0)
                return RedirectToAction("Index");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(excelFile.InputStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var fullName = worksheet.Cells[row, 1].Text?.Trim();
                    string genderText = worksheet.Cells[row, 2].Text?.Trim().ToLower();
                    bool gender = genderText == "nam";
                    var birthDateText = worksheet.Cells[row, 3].Text;
                    var phone = worksheet.Cells[row, 4].Text?.Trim();
                    var email = worksheet.Cells[row, 5].Text?.Trim();
                    var positionName = worksheet.Cells[row, 6].Text?.Trim();
                    var salaryText = worksheet.Cells[row, 7].Text?.Trim();
                    var startDateText = worksheet.Cells[row, 8].Text?.Trim();
                    var shiftName = worksheet.Cells[row, 9].Text?.Trim();
                    var userName = worksheet.Cells[row, 10].Text?.Trim();
                    DateTime.TryParseExact(startDateText, new[] { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd" },
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate);
                    
                    if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(positionName))
                        continue;

                    if (!DateTime.TryParseExact(birthDateText, new[] { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd" },
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
                        continue;

                    decimal salary = decimal.TryParse(salaryText, out var s) ? s : 0;

                    var position = new NhanVien_DAL().Select_Position_All()
                                    .FirstOrDefault(p => p.Name.Equals(positionName, StringComparison.OrdinalIgnoreCase));
                    var shift = new NhanVien_DAL().Select_CaLam_All()
               .FirstOrDefault(c => c.Name.Equals(shiftName, StringComparison.OrdinalIgnoreCase));

                    var user = new Account_DAL().Select_NguoiDung_All()
                                   .FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

                    var existing = db.NhanViens.FirstOrDefault(nv => nv.Phone == phone);
                    var username = Session["UserName"]?.ToString() ?? "Import";
                    var nhanVien = new NhanVien
                    {
                        FullName = fullName,
                        Gender = gender,
                        BirthDate = birthDate,
                        Phone = phone,
                        Email = email,
                        PositionId = position?.Id ?? 0,
                        Salary = salary,
                        StartDate = startDate,
                        ShiftId = shift?.Id ?? 0,
                        UsersId = user?.Id ?? 0
                    };
                    if (existing != null)
                    {
                        // Cập nhật
                        nhanVien.Id = existing.Id;
                        nhanVien.UpdatedDate = DateTime.Now;
                        nhanVien.UpdatedBy = username;
                        new NhanVien_DAL().Update(nhanVien);
                    }
                    else
                    {
                        // Thêm mới
                        nhanVien.CreatedDate = DateTime.Now;
                        nhanVien.CreatedBy = username;
                        nhanVien.IsDeleted = false;
                        new NhanVien_DAL().Insert(nhanVien);
                    }
                    
                }
            }

            TempData["Success"] = "Đã nhập nhân viên thành công!";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult ExportExcel(NhanVienFilter filter)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var stream = new MemoryStream();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("NhanVien");

                // Tiêu đề cột
                string[] headers = {
            "Họ tên", "Giới tính", "Ngày sinh", "Điện thoại", "Email",
            "Vị trí", "Lương", "Ngày bắt đầu", "Ca làm", "Tài khoản"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                // Dữ liệu
                var list = new NhanVien_DAL().Select_NhanVien_All(filter);

                int row = 2;
                foreach (var nv in list)
                {
                    worksheet.Cells[row, 1].Value = nv.FullName;
                    worksheet.Cells[row, 2].Value = nv.Gender ? "Nam" : "Nữ";
                    worksheet.Cells[row, 3].Value = nv.BirthDate?.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 4].Value = nv.Phone;
                    worksheet.Cells[row, 5].Value = nv.Email;
                    worksheet.Cells[row, 6].Value = nv.position?.Name;
                    worksheet.Cells[row, 7].Value = nv.Salary;
                    worksheet.Cells[row, 8].Value = nv.StartDate?.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 9].Value = nv.Shift?.Name;
                    worksheet.Cells[row, 10].Value = nv.User?.UserName;
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                package.SaveAs(stream);
            }

            stream.Position = 0;
            string fileName = "DanhSachNhanVien.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

    }
}