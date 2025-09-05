using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class PageController : Controller
    {
        // GET: Page
        public ActionResult Index()
        {
            return View();
        }
        // Hỗ trợ khách hàng
        public ActionResult FAQ() => View();
        public ActionResult Support() => View();
        public ActionResult OrderGuide() => View();
        public ActionResult Shipping() => View();
        public ActionResult ReturnPolicy() => View();

        // Về website
        public ActionResult About() => View();
        public ActionResult Privacy() => View();
        public ActionResult Terms() => View();
        public ActionResult Blog() => View();
        public ActionResult Contact() => View();
    }
}