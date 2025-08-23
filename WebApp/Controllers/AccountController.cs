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

namespace Admin.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Admin/Account
        public ActionResult Index()
        {
            var items = db.Users.ToList();
            return View(items);
        }
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "Tài khoản không tồn tại.");
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(
                user.UserName, 
                model.Password,
                model.RememberMe,
                shouldLockout: false
            );

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                default:
                    ModelState.AddModelError("", "Thông tin đăng nhập không chính xác.");
                    return View(model);
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
        public ActionResult Register(string email, string userName, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                return Json(new { code = 400, msg = "Vui lòng nhập đầy đủ thông tin!" }, JsonRequestBehavior.AllowGet);
                //ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                //return View();
            }

            if (password != confirmPassword)
            {
                return Json(new { code = 400, msg = "Mật khẩu xác nhận không khớp!" }, JsonRequestBehavior.AllowGet);
                /*ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View();*/
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                return Json(new { code = 400, msg = "Email không hợp lệ!" }, JsonRequestBehavior.AllowGet);
                /*ViewBag.Error = "Email không hợp lệ!";
                return View();*/
            }

            // Kiểm tra email hoặc username đã tồn tại chưa
            var existingUser = UserManager.Users.FirstOrDefault(u => u.Email == email || u.UserName == userName);
            if (existingUser != null)
            {
                return Json(new { code = 400, msg = "Tên đăng nhập hoặc email đã tồn tại!" }, JsonRequestBehavior.AllowGet);
                /*ViewBag.Error = "Tên đăng nhập hoặc email đã tồn tại!";
                return View();*/
            }

            // Sinh mã OTP
            string otp = new Random().Next(100000, 999999).ToString();

            TempData["OTP"] = otp;
            TempData["Email"] = email;
            TempData["UserName"] = userName;
            TempData["Password"] = password;

            // Gửi mail OTP
            bool mailSent = SendOTP(email, otp);
            if (!mailSent)
            {
                return Json(new { code = 500, msg = "Không thể gửi email xác nhận. Vui lòng thử lại!" }, JsonRequestBehavior.AllowGet);
                /*ViewBag.Error = "Không thể gửi email xác nhận. Vui lòng thử lại!";
                return View();*/
            }
            return Json(new { code = 200, msg = "Đăng ký thành công!", redirectUrl = Url.Action("ConfirmOTP") }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("ConfirmOTP");
        }

        [AllowAnonymous]
        public ActionResult ConfirmOTP()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmOTP(string otp_input)
        {
            string otp = TempData["OTP"]?.ToString();
            string email = TempData["Email"]?.ToString();
            string userName = TempData["UserName"]?.ToString();
            string password = TempData["Password"]?.ToString();

            if (otp_input == otp)
            {
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    CreatedDate = DateTime.Now,
                    CreatedBy = "System"
                };

                var result = await UserManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Đăng ký thành công!";
                    return RedirectToAction("Login");
                }
                else
                {
                    AddErrors(result);
                    TempData.Keep();
                    return View();
                }
            }

            ViewBag.Error = "Mã OTP không đúng. Vui lòng thử lại!";
            TempData.Keep();
            return View();
        }

        private bool SendOTP(string toEmail, string otp)
        {
            try
            {
                var fromAddress = new MailAddress("hoanguyen3072003@gmail.com", "Hệ thống");
                var toAddress = new MailAddress(toEmail);
                string fromPassword = "grcb nutd uzan jzqc"; // Gmail App Password
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
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }
        //
        // GET: /Account/Register
        /*[Authorize(Roles = "Admin")]*/
        public ActionResult Add(int? id)
        {
            var model = new CreateAccountViewModel();

            return PartialView("Add", model);
        }


        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Add(CreateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    Phone = model.Phone,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Json(new { code = 200, msg = "Thêm người dùng thành công" });
                }
                AddErrors(result);
            }

            return PartialView("Add", model);
        }


        //[Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            var item = UserManager.FindById(id);
            var newUser = new CreateAccountViewModel();

            if (item != null)
            {
                newUser.FullName = item.FullName;
                newUser.Email = item.Email;
                newUser.Phone = item.Phone;
                newUser.UserName = item.UserName;
                newUser.UpdatedBy = User.Identity.Name;
                newUser.UpdatedDate = DateTime.Now;
            }

            return PartialView("Add", newUser);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                }, JsonRequestBehavior.AllowGet);
            }

            //var user = await UserManager.FindByNameAsync(model.Id);
            var user = await UserManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return Json(new { code = 500, msg = "Không tìm thấy người dùng." });
            }
            user.UserName = model.UserName;
            user.FullName = model.FullName;
            user.Phone = model.Phone;
            user.Email = model.Email;
            user.UpdatedBy = User.Identity.Name;
            user.UpdatedDate = DateTime.Now;

            var result = await UserManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Json(new { code = 200, msg = "Cập nhật thành công" });

            }
            return Json(new { code = 500, msg = "Cập nhật thất bại" });
        }


        //[Authorize(Roles = "Admin")]
        public ActionResult AssignRole()
        {
            var users = db.Users.ToList();
            var model = users.Select(u => new AssignRoleViewModel
            {
                UserId = u.Id,
                UserName = u.UserName,
                FullName = u.FullName,
                Role = UserManager.GetRoles(u.Id).FirstOrDefault()
            }).ToList();

            ViewBag.AllRoles = new SelectList(db.Roles.ToList(), "Name", "Name");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<JsonResult> AssignRole(AssignRoleViewModel model)
        {
            var user = await UserManager.FindByIdAsync(model.UserId);
            if (user == null) 
                return Json(new { code = 500, msg = "Phân quyền thất bại" });

            var currentRoles = await UserManager.GetRolesAsync(user.Id);
            if (currentRoles.Any())
                await UserManager.RemoveFromRolesAsync(user.Id, currentRoles.ToArray());

            if (!string.IsNullOrEmpty(model.Role))
                await UserManager.AddToRoleAsync(user.Id, model.Role);

            return Json(new { code = 200, msg = "Phân quyền thành công" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAccount(string user, string id)
        {
            var code = new { Success = false };
            var item = UserManager.FindByName(user);
            if (item != null)
            {
                var rolesForUser = UserManager.GetRoles(id);
                if (rolesForUser != null)
                {
                    foreach (var role in rolesForUser)
                    {
                        //roles.Add(role);
                        await UserManager.RemoveFromRoleAsync(id, role);
                    }

                }

                var res = await UserManager.DeleteAsync(item);
                code = new { Success = res.Succeeded };
            }
            return Json(code);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}