using DAL;
using Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Models.XuatKho;

namespace Admin.Controllers
{
    /*[Authorize(Roles = "Admin, Employee")]*/
    public class TonKhoController : Controller
    {
        // GET: TonKho
        private DBConnect db = new DBConnect();

        public ActionResult Index(int? Month, int? Year, string searchString)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var year = Enumerable.Range(currentYear - 5, 11).Select(y => new { Id = y, Name = y.ToString() }).ToList();
            ViewBag.Years = new SelectList(year, "Id", "Name", Year);

            var months = Enumerable.Range(1, 12).Select(m => new { Id = m, Name = $"Tháng {m}" }).ToList();
            ViewBag.Months = new SelectList(months, "Id", "Name", Month);

            return View();
        }

        [HttpGet]
        public JsonResult GetTonKho(int? Month, int? Year, string searchString, int page = 1, int pageSize = 10)
        {
            List<TonKhoModel> tonkhoList = new List<TonKhoModel>();

            foreach (var p in products)
            {
                var tonKho = TonKho(p.Id); // gọi SP cho từng sản phẩm
                if (tonKho != null)
                    tonkhoList.Add(tonKho);
            }

            var pagedList = tonkhoList.OrderByDescending(x => x.SoLuongTon)
                                      .ToPagedList(page, pageSize);

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