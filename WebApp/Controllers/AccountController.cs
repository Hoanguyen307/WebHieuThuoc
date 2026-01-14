using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Models;
using WebApp;
using DAL;
using System.Configuration;

namespace Admin.Controllers
{
    public class AccountController : Controller
    {
        private DBConnect db = new DBConnect();
        //public AccountController()
        //{
        //}

        //public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //{
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //}

        //public ApplicationSignInManager SignInManager
        //{
        //    get
        //    {
        //        return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        //    }
        //    private set
        //    {
        //        _signInManager = value;
        //    }
        //}

        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}

        //// GET: Admin/Account
        //public ActionResult Index()
        //{
        //    var items = db.Users.ToList();
        //    return View(items);
        //}
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            List<KhachHang> khachhang = new KhachHang_DAL().Select_KhachHang_GetAll();
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string tendangnhap, string matkhau/*, bool RememberMe = false*/)
        {
            try
            {
                var dangnhapnd = new KhachHang_DAL().DangNhap(tendangnhap, matkhau);

                if (dangnhapnd != null)
                {
                    Session["Login"] = dangnhapnd;
                    return Json(new { code = 200, msg = "Đăng nhập thành công", redirectUrl = Url.Action("Index", "Home1") }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { code = 401, msg = "Tên đăng nhập hoặc mật khẩu không đúng" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterCustomerViewModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.PasswordHash) || string.IsNullOrEmpty(model.Address))
            {
                return Json(new { code = 400, msg = "Vui lòng nhập đầy đủ thông tin!" }, JsonRequestBehavior.AllowGet);
            }


            if (model.BirthDate == null)
                return Json(new { code = 400, msg = "Vui lòng nhập ngày sinh!" }, JsonRequestBehavior.AllowGet);


            if (!model.Email.Contains("@") || !model.Email.Contains("."))
            {
                return Json(new { code = 400, msg = "Email không hợp lệ!" }, JsonRequestBehavior.AllowGet);
            }

            var existingUser = db.KhachHangs.FirstOrDefault(u => u.Email == model.Email || u.Phone == model.Phone);
            if (existingUser != null)
            {
                return Json(new { code = 400, msg = "Số điện thoại hoặc email đã tồn tại!" }, JsonRequestBehavior.AllowGet);
            }

            // Sinh mã OTP
            string otp = new Random().Next(100000, 999999).ToString();

            Session["OTP"] = otp;
            Session["Email"] = model.Email;
            Session["Phone"] = model.Phone;
            Session["PasswordHash"] = model.PasswordHash;
            Session["FullName"] = model.FullName;
            Session["Gender"] = model.Gender;
            Session["BirthDate"] = model.BirthDate;
            Session["Address"] = model.Address;

            // Gửi mail OTP
            bool mailSent = SendOTP(model.Email, otp);
            if (!mailSent)
            {
                return Json(new { code = 500, msg = "Không thể gửi email xác nhận. Vui lòng thử lại!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { code = 200, msg = "Gửi mã xác nhận thành công!", redirectUrl = Url.Action("ConfirmOTP") }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult ConfirmOTP()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult ConfirmOTP(string otp_input)
        {
            string otp = Session["OTP"]?.ToString();
            string email = Session["Email"]?.ToString();
            string phone = Session["Phone"]?.ToString();
            string password = Session["PasswordHash"]?.ToString();
            string fullName = Session["FullName"]?.ToString();
            bool gender = Session["Gender"] != null && (bool)Session["Gender"];
            DateTime? birthDate = Session["BirthDate"] as DateTime?;
            string address = Session["Address"]?.ToString();

            if (otp_input == otp)
            {
                string hashedPassword = Common.EncryptionHelper.Encode(password);

                var customer = new KhachHang
                {
                    FullName = fullName,
                    Gender = gender,
                    BirthDate = birthDate,
                    Phone = phone,
                    Email = email,
                    Address = address,
                    PasswordHash = password,
                    CreatedBy = "System"
                };

                var dal = new KhachHang_DAL();
                int result = dal.Insert(customer);

                if (result > 0)
                {
                    return Json(new { code = 200, msg = "Đăng ký thành công!" });
                }
                else
                {
                    TempData.Keep();
                    return Json(new { code = 400, msg = "Mã OTP không đúng!" });
                }
            }

            TempData.Keep();
            return Json(new { code = 400, msg = "Mã OTP không đúng!" });
        }

        private bool SendOTP(string toEmail, string otp)
        {
            try
            {
                string fromEmail = ConfigurationManager.AppSettings["Mail_From"];
                string fromPassword = ConfigurationManager.AppSettings["Mail_Password"];
                string host = ConfigurationManager.AppSettings["Mail_Host"];
                int port = int.Parse(ConfigurationManager.AppSettings["Mail_Port"]);

                var fromAddress = new MailAddress(fromEmail, "Hệ thống");
                var toAddress = new MailAddress(toEmail);
                string subject = "Mã xác nhận đăng ký tài khoản";
                string body = $"Mã OTP của bạn là: {otp}";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                    EnableSsl = true,
                    Timeout = 20000,
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session["Login"] = null;
            return RedirectToAction("Index", "Home1");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult ForgotPassword(string email)
        {
            try
            {
                var kh = db.KhachHangs.FirstOrDefault(u => u.Email == email);
                if (kh == null)
                {
                    return Json(new { code = 404, msg = "Email không tồn tại trong hệ thống!" }, JsonRequestBehavior.AllowGet);
                }

                // Tạo mã OTP
                string otp = new Random().Next(100000, 999999).ToString();

                Session["ResetOTP"] = otp;
                Session["ResetEmail"] = email;
                Session["OtpExpire"] = DateTime.Now.AddMinutes(5);

                // Gửi OTP qua email
                bool mailSent = SendOTP(email, otp);
                if (!mailSent)
                {
                    return Json(new { code = 500, msg = "Không thể gửi email, vui lòng thử lại!" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { code = 200, msg = "Mã OTP đã được gửi về email!", redirectUrl = Url.Action("VerifyResetOtp") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public ActionResult VerifyResetOtp()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult VerifyResetOtp(string otp)
        {
            if (Session["ResetOTP"] == null || Session["OtpExpire"] == null)
                return Json(new { code = 400, msg = "OTP không tồn tại hoặc đã hết hạn" }, JsonRequestBehavior.AllowGet);

            if (DateTime.Now > (DateTime)Session["OtpExpire"])
                return Json(new { code = 401, msg = "OTP đã hết hạn" }, JsonRequestBehavior.AllowGet);

            if (otp != Session["ResetOTP"].ToString())
                return Json(new { code = 402, msg = "OTP không đúng" }, JsonRequestBehavior.AllowGet);

            return Json(new { code = 200, msg = "Xác thực OTP thành công!", redirectUrl = Url.Action("ResetPassword") }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult ResetPassword(string newPassword)
        {
            try
            {
                if (Session["ResetEmail"] == null)
                    return Json(new { code = 400, msg = "Không tìm thấy email để đặt lại mật khẩu" }, JsonRequestBehavior.AllowGet);

                string email = Session["ResetEmail"].ToString();

                var user = db.KhachHangs.FirstOrDefault(u => u.Email == email);
                if (user == null)
                    return Json(new { code = 404, msg = "Người dùng không tồn tại" }, JsonRequestBehavior.AllowGet);

                user.PasswordHash = newPassword;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                // clear session
                Session.Remove("ResetOTP");
                Session.Remove("ResetEmail");
                Session.Remove("OtpExpire");

                return Json(new { code = 200, msg = "Đặt lại mật khẩu thành công!", redirectUrl = Url.Action("Login") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}